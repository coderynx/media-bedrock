using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Application.Jobs.Messages;
using MediaBedrock.Cli.Application.Persistence;
using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.JobAssets.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.JobsStateMachine;
using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Domain.Processors;
using MediaBedrock.Cli.Domain.Processors.Interfaces;
using MediaBedrock.Sdk.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace MediaBedrock.Cli.Application.Jobs;

public sealed class JobsStateMachinesService(
    IApplicationDbContext dbContext,
    IProcessorProvider processorProvider,
    IProcessorContextFactory processorContextFactory,
    IMessageBus messageBus,
    IMediaInformationRetriever mediaInformationRetriever,
    ILogger<JobsStateMachinesService> logger) : IJobsStateMachinesService
{
    public async Task<Option<JobStateMachine>> GetAsync(JobId jobId, CancellationToken ct = default)
    {
        var jobStateMachine = await dbContext.JobsStateMachines
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .Include(j => j.StepsStateMachines)
            .ThenInclude(s => s.StepInputs)
            .Include(j => j.StepsStateMachines)
            .ThenInclude(s => s.StepOutputs)
            .SingleOrDefaultAsync(j => j.Job.Id.Equals(jobId), cancellationToken: ct);

        if (jobStateMachine is null)
        {
            return Option.None<JobStateMachine>();
        }

        logger.LogDebug("Retrieved job state machine with JobId {JobId} ", jobId);
        return Option.Some(jobStateMachine);
    }

    public async Task<List<JobStateMachine>> GetAsync(JobTemplateName jobTemplateName, CancellationToken ct = default)
    {
        var jobStateMachine = await dbContext.JobsStateMachines
            .Include(jsm => jsm.AssetsPool)
            .Include(jsm => jsm.Job)
            .ThenInclude(j => j.Template)
            .Include(jsm => jsm.StepsStateMachines)
            .ThenInclude(jssm => jssm.StepInputs)
            .Include(jsm => jsm.StepsStateMachines)
            .ThenInclude(jssm => jssm.StepOutputs)
            .Where(jsm => jsm.Job.Template.Name.Equals(jobTemplateName))
            .ToListAsync(cancellationToken: ct);

        logger.LogDebug("Retrieved job state machine with JobTemplateName {JobTemplateName} ", jobTemplateName);
        return jobStateMachine;
    }

    public async Task<Result> StartJobAsync(
        JobStateMachineId jobStateMachineId,
        CancellationToken cancellationToken = default)
    {
        var jobStateMachine = await dbContext.JobsStateMachines
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .Include(j => j.StepsStateMachines)
            .ThenInclude(s => s.StepInputs)
            .Include(j => j.StepsStateMachines)
            .ThenInclude(s => s.StepOutputs)
            .AsNoTracking()
            .SingleOrDefaultAsync(j => j.Id.Equals(jobStateMachineId), cancellationToken);

        if (jobStateMachine is null)
        {
            return JobStateMachineErrors.NotFound(jobStateMachineId);
        }

        var inputAssets = jobStateMachine.ResolveAssets(JobAssetKind.Input);

        var jobSteps = jobStateMachine.StepsStateMachines
            .Where(s => s.StepInputs.Any(a => inputAssets.Any(i => i.Name.Equals(a.AssetName))))
            .ToList();

        jobStateMachine.TransitionToRunning();
        await dbContext.SaveChangesAsync(cancellationToken);

        var processJobSteps = jobSteps.Select(jobStep => ProcessJobStep.Create(jobStateMachine.Id, jobStep.Id));
        await messageBus.PublishAsync(processJobSteps, cancellationToken);

        return Result.Accepted();
    }

    public async Task<Result> CompleteJobAsync(
        JobStateMachineId jobStateMachineId,
        CancellationToken cancellationToken = default)
    {
        var jobStateMachine = await dbContext.JobsStateMachines
            .SingleOrDefaultAsync(j => j.Id.Equals(jobStateMachineId), cancellationToken: cancellationToken);

        if (jobStateMachine is null)
        {
            return JobStateMachineErrors.NotFound(jobStateMachineId);
        }

        jobStateMachine.TransitionToCompleted();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Updated();
    }

    public async Task<Result> StartStepAsync(
        JobStateMachineId jobStateMachineId,
        JobStepStateMachineId jobStepStateMachineId,
        CancellationToken cancellationToken = default)
    {
        const string jobIdPropertyName = "JobId";
        const string jobStepNamePropertyName = "JobStepName";
        const string jobActivityIdPropertyName = "JobActivityId";
        const string processorNamePropertyName = "ProcessorName";

        logger.LogInformation("Processing job step {JobStepStateMachineId} for job execution {JobStateMachineId}",
            jobStepStateMachineId,
            jobStateMachineId);

        var jobStateMachine = await dbContext.JobsStateMachines
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .Include(j => j.StepsStateMachines)
            .ThenInclude(s => s.StepInputs)
            .Include(j => j.StepsStateMachines)
            .ThenInclude(s => s.StepOutputs)
            .SingleOrDefaultAsync(jsm => jsm.Id.Equals(jobStateMachineId), cancellationToken: cancellationToken);

        if (jobStateMachine is null)
        {
            logger.LogError("Job state machine {JobStateMachineId} not found", jobStateMachineId);
            return JobStateMachineErrors.NotFound(jobStateMachineId);
        }

        var jobStepStateMachine = jobStateMachine.StepsStateMachines
            .SingleOrDefault(ssm => ssm.Id.Equals(jobStepStateMachineId));

        if (jobStepStateMachine is null)
        {
            logger.LogError("Job step state machine {JobStepStateMachineId} not found for job {JobId}",
                jobStepStateMachineId,
                jobStateMachine.Job.Id);
            return JobStateMachineErrors.NotFound(jobStepStateMachineId);
        }

        var resolveProcessor = processorProvider.ResolveProcessor(jobStepStateMachine.ProcessorName);
        if (resolveProcessor.IsFailure)
        {
            logger.LogError("Processor {ProcessorName} not found", jobStepStateMachine.ProcessorName);
            return resolveProcessor.Error;
        }

        var createContext = processorContextFactory.Create(
            processorType: resolveProcessor.Value.GetType(),
            jobStateMachine: jobStateMachine,
            jobStepName: jobStepStateMachine.StepName);

        if (createContext.IsFailure)
        {
            logger.LogError("Failed to create job {JobId}", createContext.Error.Message);
            return createContext.Error;
        }

        var transitionToRunning = jobStepStateMachine.TransitionToRunning();
        if (transitionToRunning.IsFailure)
        {
            logger.LogError("Failed to transition job step {JobStepStateMachineId} to running: {ErrorMessage}",
                jobStepStateMachineId,
                transitionToRunning.Error.Message);

            return transitionToRunning.Error;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        using (LogContext.PushProperty(jobIdPropertyName, jobStateMachine.Job.Id))
        using (LogContext.PushProperty(jobStepNamePropertyName, jobStepStateMachine.StepName))
        using (LogContext.PushProperty(jobActivityIdPropertyName, jobStateMachine.Id))
        using (LogContext.PushProperty(processorNamePropertyName, jobStepStateMachine.ProcessorName))
        {
            var process = await Result.TryCatchAsync(
                onTry: async () =>
                {
                    var processResult = await resolveProcessor.Value.ProcessAsync(
                        context: createContext.Value,
                        ct: cancellationToken);

                    return processResult.IsSuccess
                        ? Result.Created(processResult)
                        : ProcessorErrors.ExecutionFailed(processResult.Message);
                },
                onCatch: exception => ProcessorErrors.ExecutionFailed(exception.Message));

            if (process.IsFailure)
            {
                var jobStepFailed = JobStepFailed.Create(
                    jobStateMachineId: jobStateMachineId,
                    jobStepStateMachineId: jobStepStateMachineId,
                    failureReason: JobStepFailureReason.Processing,
                    message: process.Error.Message);

                await messageBus.PublishAsync(jobStepFailed, cancellationToken);

                logger.LogError("Failed to process job step {StepName} for job {JobId}: {ErrorMessage}",
                    jobStepStateMachine.StepName,
                    jobStateMachine.Job.Id,
                    process.Error.Message);

                return process.Error;
            }
        }

        var updateAssetPool = await UpdateAssetPoolAsync(
            stateMachine: jobStateMachine,
            outputs: createContext.Value.Outputs,
            cancellationToken: cancellationToken);

        if (updateAssetPool.IsFailure)
        {
            var jobStepFailed = JobStepFailed.Create(
                jobStateMachineId: jobStateMachineId,
                jobStepStateMachineId: jobStepStateMachineId,
                failureReason: JobStepFailureReason.OutputAssetsAssessment,
                message: updateAssetPool.Error.Message);

            await messageBus.PublishAsync(jobStepFailed, cancellationToken);

            logger.LogError("Failed to update asset pool for job {JobId}: {ErrorMessage}", jobStateMachine.Job.Id,
                updateAssetPool.Error.Message);
            return updateAssetPool.Error;
        }

        var jobStepCompleted = JobStepCompleted.Create(jobStateMachineId, jobStepStateMachineId, updateAssetPool.Value);
        await messageBus.PublishAsync(jobStepCompleted, cancellationToken);

        logger.LogInformation("Successfully processed step {StepName} of job execution {JobStateMachineId}",
            jobStepStateMachine.StepName,
            jobStateMachine.Id);

        logger.LogInformation("Job {JobId} completed successfully", jobStateMachine.Job.Id);
        return Result.Accepted();
    }

    public async Task<Result> CompleteStepAsync(
        JobStepStateMachineId jobStepStateMachineId,
        List<JobAssetName> updatedAssetNames,
        CancellationToken cancellationToken = default)
    {
        var jobStepStateMachine = await dbContext.JobStepStateMachines
            .Include(jssm => jssm.JobStateMachine)
            .ThenInclude(jsm => jsm.StepsStateMachines)
            .SingleOrDefaultAsync(jssm => jssm.Id.Equals(jobStepStateMachineId), cancellationToken: cancellationToken);

        if (jobStepStateMachine is null)
        {
            return JobStateMachineErrors.NotFound(jobStepStateMachineId);
        }

        var transitionToCompleted = jobStepStateMachine.TransitionToCompleted();
        if (transitionToCompleted.IsFailure)
        {
            return transitionToCompleted.Error;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var jobStateMachine = jobStepStateMachine.JobStateMachine;
        var isJobCompleted = !jobStateMachine.StepsStateMachines
            .Any(ja => ja.ExecutionStatus.Equals(JobStepExecutionStatus.Running) ||
                       ja.ExecutionStatus.Equals(JobStepExecutionStatus.Pending));

        if (isJobCompleted)
        {
            var jobCompleted = JobCompleted.Create(jobStateMachine.Id);
            await messageBus.PublishAsync(jobCompleted, cancellationToken);
        }
        else
        {
            await PublishProcessJobStepMessagesAsync(jobStateMachine, updatedAssetNames, cancellationToken);
        }

        return Result.Updated();
    }

    public async Task<Result> FailStepAsync(
        JobStepStateMachineId jobStepStateMachineId,
        JobStepFailureReason reason,
        string message = "",
        CancellationToken cancellationToken = default)
    {
        var jobStepStateMachine = await dbContext.JobStepStateMachines
            .SingleOrDefaultAsync(j => j.Id.Equals(jobStepStateMachineId), cancellationToken: cancellationToken);

        if (jobStepStateMachine is null)
        {
            return JobStateMachineErrors.NotFound(jobStepStateMachineId);
        }

        var jobStepError = new JobStepExecutionError(reason, message);

        var transitionToFailed = jobStepStateMachine.TransitionToFailed(jobStepError);
        if (transitionToFailed.IsFailure)
        {
            return transitionToFailed.Error;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Updated();
    }

    private async Task PublishProcessJobStepMessagesAsync(
        JobStateMachine jobStateMachine,
        IEnumerable<JobAssetName> updatedAssets,
        CancellationToken ct = default)
    {
        var startJobStepMessages = jobStateMachine.StepsStateMachines
            .Where(ssm => ssm.StepInputs.Any(ss => updatedAssets.Any(ua => ua.Equals(ss.AssetName))))
            .Select(jobStep => ProcessJobStep.Create(jobStateMachine.Id, jobStep.Id));

        await messageBus.PublishAsync(startJobStepMessages, ct);
    }

    private async Task<Result<List<JobAssetName>>> UpdateAssetPoolAsync(
        JobStateMachine stateMachine,
        IReadOnlyCollection<ProcessorOutput> outputs,
        CancellationToken cancellationToken = default)
    {
        var updatedAssets = new List<JobAssetName>();

        foreach (var output in outputs)
        {
            var assetName = new JobAssetName(output.AssetName);

            var resolveAsset = stateMachine.ResolveAsset(assetName);
            if (!resolveAsset.IsSome)
            {
                return JobAssetErrors.NotFound(assetName);
            }

            var getMediaInfo = await mediaInformationRetriever.GetMediaInfoAsync(output.GetAsFilePath());
            if (getMediaInfo.IsFailure)
            {
                return getMediaInfo.Error;
            }

            var asset = resolveAsset.ValueOrThrow();
            asset.UpdateUri(output.GetAsFilePath());
            asset.MediaInformation = getMediaInfo.Value;

            updatedAssets.Add(asset.Name);

            logger.LogInformation("Asset {AssetName} became available", output.AssetName);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Created(updatedAssets);
    }

    public async Task DeleteAsync(
        JobTemplateName jobTemplateName,
        JobExecutionStatus jobExecutionStatus = JobExecutionStatus.Pending,
        CancellationToken cancellationToken = default)
    {
        var result = await dbContext.JobsStateMachines
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .ThenInclude(j => j.Template)
            .Where(j => j.Job.Template.Name.Equals(jobTemplateName) && j.ExecutionStatus.Equals(jobExecutionStatus))
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        logger.LogInformation("Deleted {JobCount} job state machines for template {JobTemplateName}",
            result,
            jobTemplateName);
    }
}
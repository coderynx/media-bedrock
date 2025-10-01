using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Application.Persistence;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobAssets.Interfaces;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.JobTemplates;
using MediaBedrock.Domain.Processors;
using MediaBedrock.Domain.Processors.Interfaces;
using MediaBedrock.Sdk.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace MediaBedrock.Application.Jobs;

public sealed class JobRunService(
    IApplicationDbContext dbContext,
    IProcessorProvider processorProvider,
    IProcessorContextFactory processorContextFactory,
    IMessageBus messageBus,
    IMediaInformationRetriever mediaInformationRetriever,
    ILogger<JobRunService> logger) : IJobRunService
{
    public async Task<Option<JobRun>> GetAsync(JobId jobId, CancellationToken ct = default)
    {
        var jobRuns = await dbContext.JobRuns
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .Include(j => j.Steps)
            .ThenInclude(s => s.StepInputs)
            .Include(j => j.Steps)
            .ThenInclude(s => s.StepOutputs)
            .SingleOrDefaultAsync(j => j.Job.Id.Equals(jobId), cancellationToken: ct);

        if (jobRuns is null)
        {
            return Option.None<JobRun>();
        }

        logger.LogDebug("Retrieved job run with JobId {JobId} ", jobId);
        return Option.Some(jobRuns);
    }
    
    public async Task<List<JobRun>> GetAsync(
        JobRunStatus runStatus,
        CancellationToken ct = default)
    {
        var jobRun = await dbContext.JobRuns
            .Include(jsm => jsm.AssetsPool)
            .Include(jsm => jsm.Job)
            .ThenInclude(j => j.Template)
            .Include(jsm => jsm.Steps)
            .ThenInclude(jssm => jssm.StepInputs)
            .Include(jsm => jsm.Steps)
            .ThenInclude(jssm => jssm.StepOutputs)
            .Where(jsm => jsm.Status.Equals(runStatus))
            .ToListAsync(cancellationToken: ct);

        logger.LogDebug("Retrieved {JobCount} job runs with execution status {ExecutionStatus}",
            jobRun.Count,
            runStatus);

        return jobRun;
    }

    public async Task<List<JobRun>> GetAsync(JobTemplateName jobTemplateName, CancellationToken ct = default)
    {
        var jobRuns = await dbContext.JobRuns
            .Include(jsm => jsm.AssetsPool)
            .Include(jsm => jsm.Job)
            .ThenInclude(j => j.Template)
            .Include(jsm => jsm.Steps)
            .ThenInclude(jssm => jssm.StepInputs)
            .Include(jsm => jsm.Steps)
            .ThenInclude(jssm => jssm.StepOutputs)
            .Where(jsm => jsm.Job.Template.Name.Equals(jobTemplateName))
            .AsNoTracking()
            .ToListAsync(cancellationToken: ct);

        logger.LogDebug("Retrieved job run with JobTemplateName {JobTemplateName} ", jobTemplateName);
        return jobRuns;
    }

    public async Task<Result> StartAsync(
        JobRunId jobRunId,
        CancellationToken cancellationToken = default)
    {
        var jobRun = await dbContext.JobRuns
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .Include(j => j.Steps)
            .ThenInclude(s => s.StepInputs)
            .Include(j => j.Steps)
            .ThenInclude(s => s.StepOutputs)
            .AsNoTracking()
            .SingleOrDefaultAsync(j => j.Id.Equals(jobRunId), cancellationToken);

        if (jobRun is null)
        {
            return JobRunErrors.NotFound(jobRunId);
        }

        var inputAssets = jobRun.ResolveAssets(JobAssetKind.Input);

        var processJobSteps = jobRun.Steps
            .Where(s => s.StepInputs.Any(a => inputAssets.Any(i => i.Name.Equals(a.AssetName))))
            .Select(jobStep => new ProcessJobStep(jobRun.Id, jobStep.Id));

        await messageBus.PublishAsync(processJobSteps, cancellationToken);

        jobRun.TransitionToRunning();
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Started job execution {JobRunId} for job {JobId}",
            jobRun.Id,
            jobRun.Job.Id);

        return Result.Accepted();
    }

    public async Task<Result> CompleteJobAsync(
        JobRunId jobRunId,
        CancellationToken cancellationToken = default)
    {
        var jobRun = await dbContext.JobRuns
            .SingleOrDefaultAsync(j => j.Id.Equals(jobRunId), cancellationToken: cancellationToken);

        if (jobRun is null)
        {
            return JobRunErrors.NotFound(jobRunId);
        }

        jobRun.TransitionToCompleted();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Updated();
    }

    public async Task<Result> StartStepAsync(
        JobRunId jobRunId,
        JobRunStepId jobRunStepId,
        CancellationToken cancellationToken = default)
    {
        const string jobIdPropertyName = "JobId";
        const string jobStepNamePropertyName = "JobStepName";
        const string jobActivityIdPropertyName = "JobActivityId";
        const string processorNamePropertyName = "ProcessorName";

        var jobRun = await dbContext.JobRuns
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .Include(j => j.Steps)
            .ThenInclude(s => s.StepInputs)
            .Include(j => j.Steps)
            .ThenInclude(s => s.StepOutputs)
            .SingleOrDefaultAsync(jsm => jsm.Id.Equals(jobRunId), cancellationToken: cancellationToken);

        if (jobRun is null)
        {
            logger.LogError("Job run {JobRunId} not found", jobRunId);
            return JobRunErrors.NotFound(jobRunId);
        }

        var jobRunStep = jobRun.Steps
            .SingleOrDefault(ssm => ssm.Id.Equals(jobRunStepId));

        if (jobRunStep is null)
        {
            logger.LogError("Job run step with id {JobRunStepId} not found for job {JobId}",
                jobRunStepId,
                jobRun.Job.Id);

            return JobRunErrors.NotFound(jobRunStepId);
        }

        var resolveProcessor = processorProvider.ResolveProcessor(jobRunStep.ProcessorName);
        if (resolveProcessor.IsFailure)
        {
            logger.LogError("Processor {ProcessorName} not found", jobRunStep.ProcessorName);
            return resolveProcessor.Error;
        }

        var createContext = processorContextFactory.Create(
            processorType: resolveProcessor.Value.GetType(),
            jobRun: jobRun,
            jobStepName: jobRunStep.StepName);

        if (createContext.IsFailure)
        {
            logger.LogError("Failed to create job {JobId}", createContext.Error.Message);
            return createContext.Error;
        }

        var transitionToRunning = jobRunStep.TransitionToRunning();
        if (transitionToRunning.IsFailure)
        {
            logger.LogError("Failed to transition job run step {JobRunStepId} to running: {ErrorMessage}",
                jobRunStepId,
                transitionToRunning.Error.Message);

            return transitionToRunning.Error;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        using (LogContext.PushProperty(jobIdPropertyName, jobRun.Job.Id))
        using (LogContext.PushProperty(jobStepNamePropertyName, jobRunStep.StepName))
        using (LogContext.PushProperty(jobActivityIdPropertyName, jobRun.Id))
        using (LogContext.PushProperty(processorNamePropertyName, jobRunStep.ProcessorName))
        {
            var processorResult = await Result.TryCatchAsync(
                onTry: async () =>
                {
                    var processResult = await resolveProcessor.Value.ProcessAsync(
                        context: createContext.Value,
                        cancellationToken: cancellationToken);

                    return processResult.IsSuccess
                        ? Result.Created(processResult)
                        : ProcessorErrors.ExecutionFailed(processResult.Message);
                },
                onCatch: exception => ProcessorErrors.ExecutionFailed(exception.Message));

            if (processorResult.IsFailure)
            {
                var jobStepFailed = new JobStepFailed(
                    jobRunId: jobRunId,
                    jobRunStepId: jobRunStepId,
                    failureReason: JobStepFailureReason.Processing,
                    message: processorResult.Error.Message);

                await messageBus.PublishAsync(jobStepFailed, cancellationToken);

                logger.LogError("Failed to process job step {StepName} for job {JobId}: {ErrorMessage}",
                    jobRunStep.StepName,
                    jobRun.Job.Id,
                    processorResult.Error.Message);

                return processorResult.Error;
            }
        }

        var updateAssetPool = await UpdateAssetPoolAsync(
            run: jobRun,
            outputs: createContext.Value.Outputs,
            cancellationToken: cancellationToken);

        if (updateAssetPool.IsFailure)
        {
            var jobStepFailed = new JobStepFailed(
                jobRunId: jobRunId,
                jobRunStepId: jobRunStepId,
                failureReason: JobStepFailureReason.OutputAssetsAssessment,
                message: updateAssetPool.Error.Message);

            await messageBus.PublishAsync(jobStepFailed, cancellationToken);

            logger.LogError("Failed to update asset pool for job {JobId}: {ErrorMessage}",
                jobRun.Job.Id,
                updateAssetPool.Error.Message);

            return updateAssetPool.Error;
        }

        var jobStepCompleted = new JobStepCompleted(jobRunId, jobRunStepId, updateAssetPool.Value);
        await messageBus.PublishAsync(jobStepCompleted, cancellationToken);

        logger.LogInformation("Successfully processed step {StepName} of job execution {JobRunId}",
            jobRunStep.StepName,
            jobRun.Id);

        logger.LogInformation("Job {JobId} completed successfully", jobRun.Job.Id);
        return Result.Accepted();
    }

    public async Task<Result> CompleteStepAsync(
        JobRunStepId jobRunStepId,
        List<JobAssetName> updatedAssetNames,
        CancellationToken cancellationToken = default)
    {
        var jobRunSteps = await dbContext.JobRunSteps
            .Include(jssm => jssm.JobRun)
            .ThenInclude(jsm => jsm.Steps)
            .SingleOrDefaultAsync(jssm => jssm.Id.Equals(jobRunStepId), cancellationToken: cancellationToken);

        if (jobRunSteps is null)
        {
            return JobRunErrors.NotFound(jobRunStepId);
        }

        var transitionToCompleted = jobRunSteps.TransitionToCompleted();
        if (transitionToCompleted.IsFailure)
        {
            return transitionToCompleted.Error;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var jobRun = jobRunSteps.JobRun;
        var isJobCompleted = !jobRun.Steps
            .Any(ja => ja.Status.Equals(JobRunStepStatus.Running) ||
                       ja.Status.Equals(JobRunStepStatus.Pending));

        if (isJobCompleted)
        {
            var jobCompleted = new JobCompleted(jobRun.Id);
            await messageBus.PublishAsync(jobCompleted, cancellationToken);
        }
        else
        {
            await PublishProcessJobStepMessagesAsync(jobRun, updatedAssetNames, cancellationToken);
        }

        return Result.Updated();
    }

    public async Task<Result> FailStepAsync(
        JobRunStepId jobRunStepId,
        JobStepFailureReason reason,
        string message = "",
        CancellationToken cancellationToken = default)
    {
        var jobRunStep = await dbContext.JobRunSteps
            .Include(jssm => jssm.JobRun)
            .SingleOrDefaultAsync(j => j.Id.Equals(jobRunStepId), cancellationToken: cancellationToken);

        if (jobRunStep is null)
        {
            return JobRunErrors.NotFound(jobRunStepId);
        }

        var jobStepError = new JobRunStepError(reason, message);

        var transitionToFailed = jobRunStep.TransitionToFailed(jobStepError);
        if (transitionToFailed.IsFailure)
        {
            return transitionToFailed.Error;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        // TODO: Implement failure reason creation.
        var jobFailed = new JobFailed(
            jobRunStep.JobRun.Id,
            JobFailureReason.Processing,
            message);

        await messageBus.PublishAsync(jobFailed, cancellationToken);

        return Result.Updated();
    }

    public async Task DeleteAsync(
        JobTemplateName jobTemplateName,
        JobRunStatus jobRunStatus = JobRunStatus.Pending,
        CancellationToken cancellationToken = default)
    {
        var jobRunCount = await dbContext.JobRuns
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .ThenInclude(j => j.Template)
            .Where(j => j.Job.Template.Name.Equals(jobTemplateName) && j.Status.Equals(jobRunStatus))
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        logger.LogInformation("Deleted {JobRunCount} job runs for template {JobTemplateName}",
            jobRunCount,
            jobTemplateName);
    }

    public async Task<Result> FailJobAsync(
        JobRunId jobRunId,
        JobFailureReason failureReason,
        string message,
        CancellationToken cancellationToken)
    {
        var jobRun = await dbContext.JobRuns
            .SingleOrDefaultAsync(j => j.Id.Equals(jobRunId), cancellationToken);

        if (jobRun is null)
        {
            return JobRunErrors.NotFound(jobRunId);
        }

        jobRun.TransitionToFailed(failureReason, message);

        return await dbContext.SaveChangesAsync(cancellationToken) > 0
            ? Result.Updated()
            : JobRunErrors.UpdateFailed(jobRunId);
    }

    private async Task PublishProcessJobStepMessagesAsync(
        JobRun jobRun,
        IEnumerable<JobAssetName> updatedAssets,
        CancellationToken ct = default)
    {
        var startJobStepMessages = jobRun.Steps
            .Where(ssm => ssm.StepInputs.Any(ss => updatedAssets.Any(ua => ua.Equals(ss.AssetName))))
            .Select(jobStep => new ProcessJobStep(jobRun.Id, jobStep.Id));

        await messageBus.PublishAsync(startJobStepMessages, ct);
    }

    private async Task<Result<List<JobAssetName>>> UpdateAssetPoolAsync(
        JobRun run,
        IReadOnlyCollection<ProcessorOutput> outputs,
        CancellationToken cancellationToken = default)
    {
        var updatedAssets = new List<JobAssetName>();

        foreach (var output in outputs)
        {
            var assetName = new JobAssetName(output.AssetName);

            var resolveAsset = run.ResolveAsset(assetName);
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
}
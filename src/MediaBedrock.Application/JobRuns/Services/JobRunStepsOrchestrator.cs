using Coderynx.Functional.Results;
using MediaBedrock.Application.Database;
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.JobRuns.Messages;
using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobAssets.Interfaces;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Processors;
using MediaBedrock.Domain.Processors.Interfaces;
using MediaBedrock.Sdk.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace MediaBedrock.Application.JobRuns.Services;

public sealed class JobRunStepsOrchestrator(
    IApplicationDbContext dbContext,
    IMessageBus messageBus,
    IProcessorProvider processorProvider,
    IProcessorContextFactory processorContextFactory,
    IMediaInformationRetriever mediaInformationRetriever,
    ILogger<JobRunStepsOrchestrator> logger) : IJobRunStepsOrchestrator
{
    public async Task<Result> StartAsync(
        JobRunId jobRunId,
        JobRunStepId jobRunStepId,
        CancellationToken cancellationToken = new())
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
                var jobStepFailed = new JobRunStepFailed(
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
            var jobStepFailed = new JobRunStepFailed(
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

        var jobStepCompleted = new JobRunStepCompleted(jobRunId, jobRunStepId, updateAssetPool.Value);
        await messageBus.PublishAsync(jobStepCompleted, cancellationToken);

        logger.LogInformation("Successfully processed step {StepName} of job execution {JobRunId}",
            jobRunStep.StepName,
            jobRun.Id);

        logger.LogInformation("Job {JobId} completed successfully", jobRun.Job.Id);
        return Result.Accepted();
    }

    public async Task<Result> CompleteAsync(
        JobRunStepId jobRunStepId,
        List<JobAssetName> updatedAssetNames,
        CancellationToken cancellationToken = new())
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
            var jobCompleted = new JobRunCompleted(jobRun.Id);
            await messageBus.PublishAsync(jobCompleted, cancellationToken);
        }
        else
        {
            var startJobStepMessages = jobRun.Steps
                .Where(ssm => ssm.StepInputs.Any(ss => updatedAssetNames.Any(ua => ua.Equals(ss.AssetName))))
                .Select(jobStep => new ProcessJobRunStep(jobRun.Id, jobStep.Id));

            await messageBus.PublishAsync(startJobStepMessages, cancellationToken);
        }

        return Result.Updated();
    }

    public async Task<Result> FailAsync(
        JobRunStepId jobRunStepId,
        JobStepFailureReason reason,
        string message = "",
        CancellationToken cancellationToken = new())
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
        var jobFailed = new JobRunFailed(
            jobRunStep.JobRun.Id,
            JobFailureReason.Processing,
            message);

        await messageBus.PublishAsync(jobFailed, cancellationToken);

        return Result.Updated();
    }

    private async Task<Result<List<JobAssetName>>> UpdateAssetPoolAsync(
        JobRun run,
        IReadOnlyCollection<ProcessorOutput> outputs,
        CancellationToken cancellationToken = new())
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
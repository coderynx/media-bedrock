using Coderynx.Functional.Results;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Application.Persistence;
using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.JobAssets.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.Processors.Interfaces;
using MediaBedrock.Sdk.Processors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace MediaBedrock.Cli.Application.Jobs.Handlers;

public sealed record ProcessJobStep : JobMessage
{
    public required JobStepName StepName { get; init; }
}

public sealed class ProcessJobStepHandler(
    IProcessorContextFactory processorContextFactory,
    IProcessorProvider processorProvider,
    IMediaInformationRetriever mediaInformationRetriever,
    IJobMessageBus messageBus,
    IApplicationDbContext dbContext,
    ILogger<ProcessJobStepHandler> logger)
    : IJobMessageHandler<ProcessJobStep>
{
    public async Task HandleAsync(ProcessJobStep message, CancellationToken ct = default)
    {
        const string jobIdPropertyName = "JobId";
        const string jobStepNamePropertyName = "JobStepName";
        const string jobActivityIdPropertyName = "JobActivityId";
        const string processorNamePropertyName = "ProcessorName";

        logger.LogInformation("Processing job step {StepName} for job {JobId}",
            message.StepName,
            message.JobId);

        var jobStateMachine = dbContext.JobsStateMachines
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .Include(j => j.StepsStateMachines)
            .ThenInclude(s => s.StepSinks)
            .Include(j => j.StepsStateMachines)
            .ThenInclude(s => s.StepSources)
            .SingleOrDefault(ja => ja.Job.Id.Equals(message.JobId));

        if (jobStateMachine is null)
        {
            logger.LogError("Job state machine for Job {JobId} not found", message.JobId);
            return;
        }

        var jobStepStateMachine = jobStateMachine.StepsStateMachines
            .SingleOrDefault(ja => ja.StepName.Equals(message.StepName));

        if (jobStepStateMachine is null)
        {
            logger.LogError("Job step state machine for job {JobId} not found", message.JobId);
            return;
        }

        var resolveProcessor = processorProvider.ResolveProcessor(jobStepStateMachine.ProcessorName);
        if (resolveProcessor.IsFailure)
        {
            logger.LogError("Processor {ProcessorName} not found for job {JobId}",
                jobStepStateMachine.ProcessorName,
                message.JobId);
            return;
        }

        var createContext = processorContextFactory.Create(
            processorType: resolveProcessor.Value.GetType(),
            jobStateMachine: jobStateMachine,
            jobStepName: message.StepName);

        if (createContext.IsFailure)
        {
            logger.LogError("Failed to create job {JobId}", createContext.Error.Message);
            return;
        }

        jobStepStateMachine.TransitionToRunning();
        await dbContext.SaveChangesAsync(ct);

        using (LogContext.PushProperty(jobIdPropertyName, jobStateMachine.Job.Id))
        using (LogContext.PushProperty(jobStepNamePropertyName, jobStepStateMachine.StepName))
        using (LogContext.PushProperty(jobActivityIdPropertyName, jobStateMachine.Id))
        using (LogContext.PushProperty(processorNamePropertyName, jobStepStateMachine.ProcessorName))
        {
            try
            {
                var processResult = await resolveProcessor.Value.ProcessAsync(createContext.Value, ct);
                if (!processResult.IsSuccess)
                {
                    jobStepStateMachine.TransitionToFailed();
                    await dbContext.SaveChangesAsync(ct);

                    logger.LogError("Failed to process job step {StepName} for job {JobId}: {ErrorMessage}",
                        jobStepStateMachine.StepName,
                        jobStateMachine.Job.Id,
                        processResult.Message);
                    return;
                }
            }
            catch (Exception e)
            {
                jobStepStateMachine.TransitionToFailed();
                await dbContext.SaveChangesAsync(ct);

                logger.LogError(e, "Failed to process job step {StepName} for job {JobId}",
                    jobStepStateMachine.StepName,
                    jobStateMachine.Job.Id);

                return;
            }
        }

        var updateAssetPool = await UpdateAssetPoolAsync(jobStateMachine, createContext.Value.Outputs);
        if (updateAssetPool.IsFailure)
        {
            jobStepStateMachine.TransitionToFailed();

            logger.LogError("Failed to update asset pool for job {JobId}", updateAssetPool.Error.Message);
            return;
        }

        jobStepStateMachine.TransitionToCompleted();
        await dbContext.SaveChangesAsync(ct);

        await PublishProcessJobStepMessagesAsync(jobStateMachine, updateAssetPool.Value, ct);

        logger.LogInformation("Successfully processed step {StepName} of job {JobId}",
            jobStepStateMachine.StepName,
            jobStateMachine.Id);

        var isJobCompleted = !jobStateMachine.StepsStateMachines
            .Any(ja => ja.Status.Equals(JobStepStatus.Running) || ja.Status.Equals(JobStepStatus.Pending));

        if (isJobCompleted)
        {
            jobStateMachine.TransitionToCompleted();
            await dbContext.SaveChangesAsync(ct);

            logger.LogInformation("Job {JobId} completed successfully", jobStateMachine.Job.Id);
        }
    }

    private async Task PublishProcessJobStepMessagesAsync(
        JobStateMachine jobStateMachine,
        List<JobAssetName> updatedAssets,
        CancellationToken ct = default)
    {
        var stepsStateMachines = jobStateMachine.StepsStateMachines
            .Where(ssm => ssm.StepSinks.Any(ss => updatedAssets.Any(ua => ua.Equals(ss.AssetName))))
            .ToList();

        foreach (var jobStep in stepsStateMachines)
        {
            var startJobStep = new ProcessJobStep
            {
                JobId = jobStateMachine.Job.Id,
                StepName = jobStep.StepName
            };

            await messageBus.PublishAsync(startJobStep, ct);
        }
    }

    private async Task<Result<List<JobAssetName>>> UpdateAssetPoolAsync(
        JobStateMachine stateMachine,
        IReadOnlyCollection<ProcessorOutput> outputs)
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

        return Result.Created(updatedAssets);
    }
}
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Application.Assets;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Assets;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

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
    ILogger<ProcessJobStepHandler> logger)
    : IJobMessageHandler<ProcessJobStep>
{
    public async Task HandleAsync(JobMessageContext<ProcessJobStep> context, CancellationToken ct = default)
    {
        var jobActivity = context.JobStateMachine.JobActivities
            .SingleOrDefault(ja => ja.Step.Name.Equals(context.JobMessage.StepName));

        if (jobActivity is null)
        {
            logger.LogError("Job step {StepName} not found in job {JobId}",
                context.JobMessage.StepName,
                context.JobMessage.JobId);
            return;
        }

        var createContext = processorContextFactory.Create(
            jobId: context.JobMessage.JobId,
            step: jobActivity.Step,
            assetsPool: context.JobStateMachine.AssetsPool);

        if (createContext.IsFailure)
        {
            logger.LogError("Failed to create job {JobId}", createContext.Error.Message);
            return;
        }

        var resolveProcessor = processorProvider.ResolveProcessor(jobActivity.Step.ProcessorName);
        if (resolveProcessor.IsFailure)
        {
            logger.LogError("Processor {ProcessorName} not found for job {JobId}",
                jobActivity.Step.ProcessorName,
                context.JobMessage.JobId);
            return;
        }

        jobActivity.UpdateStatus(JobActivityStatus.Running);

        try
        {
            var processResult = await resolveProcessor.Value.ProcessAsync(createContext.Value, ct);
            if (!processResult.IsSuccess)
            {
                jobActivity.UpdateStatus(JobActivityStatus.Failed);
                logger.LogError("Failed to process job step {StepName} for job {JobId}: {ErrorMessage}",
                    context.JobMessage.StepName,
                    context.JobMessage.JobId,
                    processResult.Message);
                return;
            }
        }
        catch (Exception e)
        {
            jobActivity.UpdateStatus(JobActivityStatus.Failed);
            logger.LogError(e, "Failed to process job step {StepName} for job {JobId}",
                context.JobMessage.StepName,
                context.JobMessage.JobId);
            return;
        }

        var updateAssetPool = await UpdateAssetPoolAsync(context.JobStateMachine, createContext.Value);
        if (updateAssetPool.IsFailure)
        {
            return;
        }

        jobActivity.UpdateStatus(JobActivityStatus.Completed);

        await PublishProcessJobStepMessagesAsync(context, updateAssetPool.Value, ct);

        logger.LogInformation("Successfully processed step {StepName} of job {JobId}",
            context.JobMessage.StepName,
            context.JobMessage.JobId);

        var isJobCompleted = !context.JobStateMachine.JobActivities
            .Any(ja => ja.Status.Equals(JobActivityStatus.Running) || ja.Status.Equals(JobActivityStatus.Pending));

        if (isJobCompleted)
        {
            context.JobStateMachine.UpdateStatus(JobWorkflowStatus.Completed);
            logger.LogInformation("Job {JobId} completed successfully", context.JobMessage.JobId);
        }
    }

    private async Task PublishProcessJobStepMessagesAsync(
        JobMessageContext<ProcessJobStep> context,
        List<string> updatedAssets,
        CancellationToken ct = default)
    {
        var jobSteps = updatedAssets.Select(updatedJobAsset =>
            context.JobStateMachine.JobActivities.SingleOrDefault(ja =>
                ja.Step.Sinks.Any(a => a.AssetName.Equals(updatedJobAsset))));

        foreach (var activity in jobSteps)
        {
            if (activity is null)
            {
                logger.LogInformation("Skipping job step {StepName} for job {JobId} as no assets were updated",
                    context.JobMessage.StepName,
                    context.JobMessage.JobId);
                continue;
            }

            var startJobStep = new ProcessJobStep
            {
                JobId = context.JobMessage.JobId,
                StepName = activity.Step.Name
            };

            await messageBus.PublishAsync(startJobStep, ct);
        }
    }

    private async Task<Result<List<string>>> UpdateAssetPoolAsync(JobStateMachine stateMachine,
        ProcessorContext context)
    {
        var updatedAssets = new List<string>();

        foreach (var output in context.Outputs)
        {
            var resolveAsset = stateMachine.AssetsPool.ResolveAsset(output.AssetName);
            if (!resolveAsset.IsSome)
            {
                return JobAssetErrors.AssetNotFound(output.AssetName);
            }

            var getMediaInfo = await mediaInformationRetriever.GetMediaInfoAsync(output.GetAsFilePath());
            if (getMediaInfo.IsFailure)
            {
                return getMediaInfo.Error;
            }

            var asset = resolveAsset.ValueOrThrow();
            asset.UpdateUri(output.GetAsFilePath());

            updatedAssets.Add(asset.Name);

            logger.LogInformation("Asset {AssetName} became available", output.AssetName);
        }

        return Result.Created(updatedAssets);
    }
}
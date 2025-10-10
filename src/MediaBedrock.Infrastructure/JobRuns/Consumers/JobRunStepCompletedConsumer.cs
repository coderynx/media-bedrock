using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobRuns;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class JobRunStepCompletedConsumer(
    IJobRunStepsOrchestrator jobRunStepsOrchestrator,
    ILogger<JobRunStepCompletedConsumer> logger)
    : IConsumer<JobRunStepCompleted>
{
    public async Task ConsumeAsync(ConsumerContext<JobRunStepCompleted> context, CancellationToken ct = new())
    {
        var message = context.Message;

        var createJobRunStepId = JobRunStepId.Create(message.JobRunStepId);
        if (createJobRunStepId.IsFailure)
        {
            logger.LogError("Failed to create job run step id for job {JobId}", message.JobRunId);
            return;
        }

        var createUpdatedAssetNames = message.UpdatedAssetNames
            .Select(JobAssetName.Create)
            .ToList();

        if (createUpdatedAssetNames.Any(x => x.IsFailure))
        {
            logger.LogError(
                "Failed to create updated asset names for job run step {JobRunStepId} for job run {JobRunId}. Reason: {ErrorMessage}",
                message.JobRunStepId,
                message.JobRunId,
                createUpdatedAssetNames.First(x => x.IsFailure).Error.Message);

            return;
        }

        var completeStep = await jobRunStepsOrchestrator.CompleteAsync(
            jobRunStepId: createJobRunStepId.Value,
            updatedAssetNames: createUpdatedAssetNames.Select(x => x.Value).ToList(),
            cancellationToken: ct);

        if (completeStep.IsFailure)
        {
            logger.LogError("Failed to complete job step {JobRunStepId} for job run {JobRunId}. Reason: {ErrorMessage}",
                message.JobRunStepId,
                message.JobRunId,
                completeStep.Error.Message);
        }
    }
}
using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class JobRunStepCompletedConsumer(IJobRunStepsOrchestrator jobRunStepsOrchestrator)
    : IConsumer<JobRunStepCompleted>
{
    public async Task ConsumeAsync(ConsumerContext<JobRunStepCompleted> context, CancellationToken ct = new())
    {
        var message = context.Message;

        var createJobRunStepId = JobRunStepId.Create(message.JobRunId);
        if (createJobRunStepId.IsFailure)
        {
            return;
        }

        var createUpdatedAssetNames = message.UpdatedAssetNames
            .Select(JobAssetName.Create)
            .ToList();

        if (createUpdatedAssetNames.Any(x => x.IsFailure))
        {
            return;
        }

        var completeStep = await jobRunStepsOrchestrator.CompleteAsync(
            jobRunStepId: createJobRunStepId.Value,
            updatedAssetNames: createUpdatedAssetNames.Select(x => x.Value).ToList(),
            cancellationToken: ct);

        if (completeStep.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to stop job step {message.JobRunStepId} for job {message.JobRunId}");
        }
    }
}
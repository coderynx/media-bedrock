using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.JobRuns.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class JobRunStepCompletedConsumer(IJobRunStepsOrchestrator jobRunStepsOrchestrator)
    : IMessageConsumer<JobRunStepCompleted>
{
    public async Task HandleAsync(JobRunStepCompleted message, CancellationToken cancellationToken = default)
    {
        var completeStep = await jobRunStepsOrchestrator.CompleteAsync(
            jobRunStepId: message.JobRunStepId,
            updatedAssetNames: message.UpdatedAssetNames,
            cancellationToken: cancellationToken);

        if (completeStep.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to stop job step {message.JobRunStepId} for job {message.JobRunId}");
        }
    }
}
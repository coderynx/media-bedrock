using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class JobStepCompletedConsumer(IJobRunStepsOrchestrator jobRunStepsOrchestrator)
    : IMessageConsumer<JobStepCompleted>
{
    public async Task HandleAsync(JobStepCompleted message, CancellationToken cancellationToken = default)
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
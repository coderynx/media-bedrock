using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class JobStepCompletedConsumer(IJobRunService jobRunService)
    : IMessageConsumer<JobStepCompleted>
{
    public async Task HandleAsync(JobStepCompleted message, CancellationToken cancellationToken = default)
    {
        var stop = await jobRunService.CompleteStepAsync(
            jobRunStepId: message.JobRunStepId,
            updatedAssetNames: message.UpdatedAssetNames,
            cancellationToken: cancellationToken);

        if (stop.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to stop job step {message.JobRunStepId} for job {message.JobRunId}");
        }
    }
}
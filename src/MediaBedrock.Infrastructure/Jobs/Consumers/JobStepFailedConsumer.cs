using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class JobStepFailedConsumer(IJobRunService jobRunService)
    : IMessageConsumer<JobStepFailed>
{
    public async Task HandleAsync(JobStepFailed message, CancellationToken cancellationToken = default)
    {
        var stop = await jobRunService.FailStepAsync(
            jobRunStepId: message.JobRunStepId,
            reason: message.FailureReason,
            message: message.Message,
            cancellationToken: cancellationToken);

        if (stop.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to stop job step {message.JobRunStepId} for job {message.JobRunId}");
        }
    }
}
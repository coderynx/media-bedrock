using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.JobRuns.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class JobRunFailedConsumer(IJobRunsOrchestrator jobRunsOrchestrator) : IMessageConsumer<JobRunFailed>
{
    public async Task HandleAsync(JobRunFailed message, CancellationToken cancellationToken = default)
    {
        var failJob = await jobRunsOrchestrator.FailAsync(
            jobRunId: message.JobRunId,
            failureReason: message.FailureReason,
            message: message.Message,
            cancellationToken: cancellationToken);

        if (failJob.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to fail job {message.JobRunId} with reason {message.FailureReason}");
        }
    }
}
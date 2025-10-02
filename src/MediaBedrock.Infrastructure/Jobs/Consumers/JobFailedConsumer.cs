using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class JobFailedConsumer(IJobRunsOrchestrator jobRunsOrchestrator) : IMessageConsumer<JobFailed>
{
    public async Task HandleAsync(JobFailed message, CancellationToken cancellationToken = default)
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
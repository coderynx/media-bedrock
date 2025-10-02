using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.JobRuns.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class JobRunCompletedConsumer(IJobRunsOrchestrator jobRunsOrchestrator)
    : IMessageConsumer<JobRunCompleted>
{
    public async Task HandleAsync(JobRunCompleted message, CancellationToken cancellationToken = default)
    {
        var completeJob = await jobRunsOrchestrator.CompleteAsync(
            jobRunId: message.JobRunId,
            cancellationToken: cancellationToken);

        if (completeJob.IsFailure)
        {
            throw new InvalidOperationException($"Failed to complete job {message.JobRunId}");
        }
    }
}
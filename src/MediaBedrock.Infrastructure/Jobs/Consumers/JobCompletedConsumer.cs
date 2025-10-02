using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class JobCompletedConsumer(IJobRunsOrchestrator jobRunsOrchestrator)
    : IMessageConsumer<JobCompleted>
{
    public async Task HandleAsync(JobCompleted message, CancellationToken cancellationToken = default)
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
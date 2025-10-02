using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.JobRuns.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class SartJobRunConsumer(IJobRunsOrchestrator jobRunsOrchestrator) : IMessageConsumer<StartJobRun>
{
    public async Task HandleAsync(StartJobRun message, CancellationToken cancellationToken = new())
    {
        var startJob = await jobRunsOrchestrator.StartAsync(message.JobRunId, cancellationToken);
        if (startJob.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to run job {message.JobRunId}");
        }
    }
}
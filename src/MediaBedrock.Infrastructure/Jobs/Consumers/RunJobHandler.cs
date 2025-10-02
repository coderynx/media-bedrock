using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class RunConsumer(IJobRunsOrchestrator jobRunsOrchestrator) : IMessageConsumer<RunJob>
{
    public async Task HandleAsync(RunJob message, CancellationToken cancellationToken = new())
    {
        var startJob = await jobRunsOrchestrator.StartAsync(message.JobRunId, cancellationToken);
        if (startJob.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to run job {message.JobRunId}");
        }
    }
}
using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class RunConsumer(IJobRunService jobRunService) : IMessageConsumer<RunJob>
{
    public async Task HandleAsync(RunJob message, CancellationToken cancellationToken = default)
    {
        var run = await jobRunService.StartAsync(message.JobRunId, cancellationToken);
        if (run.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to run job {message.JobRunId}");
        }
    }
}
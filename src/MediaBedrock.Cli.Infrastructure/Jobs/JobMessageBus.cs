using MediaBedrock.Cli.Application.Jobs;
using MediaBedrock.Cli.Application.Jobs.Interfaces;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

internal sealed class JobMessageBus(JobMessageQueue queue) : IJobMessageBus
{
    public async Task PublishAsync<TJobEvent>(TJobEvent jobEvent, CancellationToken cancellationToken = default)
        where TJobEvent : JobMessage
    {
        await queue.Writer.WriteAsync(jobEvent, cancellationToken);
    }
}
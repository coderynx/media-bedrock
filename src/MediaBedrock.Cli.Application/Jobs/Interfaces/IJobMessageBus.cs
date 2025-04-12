namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IJobMessageBus
{
    Task PublishAsync<TJobEvent>(TJobEvent jobEvent, CancellationToken cancellationToken = default)
        where TJobEvent : JobMessage;
}
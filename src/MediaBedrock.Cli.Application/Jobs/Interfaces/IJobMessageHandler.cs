namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IJobMessageHandler<in TJobMessage> where TJobMessage : JobMessage
{
    Task HandleAsync(TJobMessage message, CancellationToken ct = default);
}
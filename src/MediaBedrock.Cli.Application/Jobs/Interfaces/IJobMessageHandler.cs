namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IJobMessageHandler<TJobMessage> where TJobMessage : JobMessage
{
    Task HandleAsync(JobMessageContext<TJobMessage> context, CancellationToken ct = default);
}
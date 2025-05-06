using MediaBedrock.Cli.Application.Jobs.Messages;

namespace MediaBedrock.Cli.Infrastructure.Messaging;

public interface IMessageHandler<in TJobMessage> where TJobMessage : JobMessageBase
{
    Task HandleAsync(TJobMessage message, CancellationToken cancellationToken = default);
}
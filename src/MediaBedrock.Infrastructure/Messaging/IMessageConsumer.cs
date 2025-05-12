using MediaBedrock.Application.Jobs.Interfaces;

namespace MediaBedrock.Infrastructure.Messaging;

public interface IMessageConsumer<in TMessage> where TMessage : IMessage
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}
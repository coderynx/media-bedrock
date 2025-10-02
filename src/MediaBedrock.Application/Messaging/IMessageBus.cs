using MediaBedrock.Contracts;

namespace MediaBedrock.Application.Messaging;

public interface IMessageBus
{
    Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = new())
        where TMessage : IMessage;

    Task PublishAsync<TMessage>(IEnumerable<TMessage> messages, CancellationToken cancellationToken = new())
        where TMessage : IMessage;
}
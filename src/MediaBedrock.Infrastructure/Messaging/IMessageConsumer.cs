using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Contracts;

namespace MediaBedrock.Infrastructure.Messaging;

public interface IMessageConsumer<in TMessage> where TMessage : IMessage
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken = default);
}
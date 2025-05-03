using MediaBedrock.Cli.Application.Jobs.Interfaces;

namespace MediaBedrock.Cli.Infrastructure.Messaging;

internal sealed class InMemoryMessageBus(InMemoryMessageQueue queue) : IMessageBus
{
    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : IMessage
    {
        await queue.Writer.WriteAsync(message, cancellationToken);
    }

    public Task PublishAsync<TMessage>(
        IEnumerable<TMessage> messages,
        CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        var tasks = messages.Select(message => queue.Writer.WriteAsync(message, cancellationToken).AsTask());
        return Task.WhenAll(tasks);
    }
}
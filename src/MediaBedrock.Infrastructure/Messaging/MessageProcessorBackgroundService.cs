using MediaBedrock.Application.Jobs.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Infrastructure.Messaging;

internal sealed class MessageProcessorBackgroundService(
    InMemoryMessageQueue queue,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<MessageProcessorBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in queue.Reader.ReadAllAsync(stoppingToken))
        {
            // TODO: Use source generators for resolving handlers.
            var scope = serviceScopeFactory.CreateScope();

            var handlerType = typeof(IMessageConsumer<>).MakeGenericType(message.GetType());
            var handler = scope.ServiceProvider.GetService(handlerType);

            if (handler is null)
            {
                logger.LogError("Failed to resolve handler for message {MessageType}", message.GetType());
                continue;
            }

            const string methodName = nameof(IMessageConsumer<IMessage>.HandleAsync);

            var handleMethod = handlerType.GetMethod(methodName);
            if (handleMethod is null)
            {
                logger.LogError("Failed to resolve handle method for handler {HandlerType}", handlerType);
                continue;
            }

            if (handleMethod.Invoke(handler, [message, stoppingToken]) is not Task task)
            {
                continue;
            }

            logger.LogInformation("Handling message {MessageType}", message.GetType());

            _ = Task.Factory.StartNew(async () =>
            {
                await task;
                scope.Dispose();
            }, TaskCreationOptions.LongRunning);
        }
    }
}
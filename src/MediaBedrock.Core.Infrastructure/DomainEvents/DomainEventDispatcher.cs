using System.Collections.Concurrent;
using MediaBedrock.Core.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Core.Infrastructure.DomainEvents;

public sealed class DomainEventsDispatcher(IServiceProvider serviceProvider) : IDomainEventsDispatcher
{
    private static readonly ConcurrentDictionary<Type, Type> HandlerTypeDictionary = new();
    private static readonly ConcurrentDictionary<Type, Type> WrapperTypeDictionary = new();

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = new())
    {
        foreach (var domainEvent in domainEvents)
        {
            using var scope = serviceProvider.CreateScope();

            var domainEventType = domainEvent.GetType();
            var handlerType = HandlerTypeDictionary.GetOrAdd(
                domainEventType,
                et => typeof(IDomainEventConsumer<>).MakeGenericType(et));

            var handlers = scope.ServiceProvider.GetServices(handlerType);

            foreach (var handler in handlers)
            {
                if (handler is null)
                {
                    continue;
                }

                var handlerWrapper = HandlerWrapper.Create(handler, domainEventType);
                if (handlerWrapper is null)
                {
                    continue;
                }

                await handlerWrapper.ConsumeAsync(domainEvent, cancellationToken);
            }
        }
    }

    private abstract class HandlerWrapper
    {
        public abstract Task ConsumeAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = new());

        public static HandlerWrapper? Create(object handler, Type domainEventType)
        {
            var wrapperType = WrapperTypeDictionary.GetOrAdd(
                domainEventType,
                et => typeof(HandlerWrapper<>).MakeGenericType(et));

            return Activator.CreateInstance(wrapperType, handler) as HandlerWrapper;
        }
    }

    private sealed class HandlerWrapper<TDomainEvent>(object handler) : HandlerWrapper where TDomainEvent : IDomainEvent
    {
        private readonly IDomainEventConsumer<TDomainEvent> _handler = (IDomainEventConsumer<TDomainEvent>)handler;

        public override async Task ConsumeAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = new())
        {
            await _handler.ConsumeAsync((TDomainEvent)domainEvent, cancellationToken);
        }
    }
}
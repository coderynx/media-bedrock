using MediaBedrock.Core.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Core.Infrastructure.DomainEvents;

public static class DependencyInjection
{
    public static void AddDomainEvent<TDomainEvent, TDomainEventConsumer>(this IServiceCollection services)
        where TDomainEvent : IDomainEvent
        where TDomainEventConsumer : class, IDomainEventConsumer<TDomainEvent>
    {
        services.AddScoped<IDomainEventConsumer<TDomainEvent>, TDomainEventConsumer>();
    }
    
    public static void AddDomainEvents(this IServiceCollection services)
    {
        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();
    }
}
using MediaBedrock.Application.JobRuns.DomainEventConsumers;
using MediaBedrock.Domain.Abstractions;
using MediaBedrock.Domain.JobRuns.DomainEvents;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Infrastructure.DomainEvents;

public static class DependencyInjection
{
    public static void AddDomainEvents(this IServiceCollection services)
    {
        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        services.AddScoped<IDomainEventConsumer<JobRunStartedDomainEvent>, JobRunStartedDomainEventConsumer>();
    }
}
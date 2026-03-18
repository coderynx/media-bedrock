using Coderynx.MessagingKit;
using Coderynx.MessagingKit.Transports.InMemory;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Contracts.Processing;
using MediaBedrock.Controller.Application.JobRuns.DomainEventConsumers;
using MediaBedrock.Controller.Domain.JobRuns.DomainEvents;
using MediaBedrock.Controller.Infrastructure.Database.Bootstrap;
using MediaBedrock.Controller.Infrastructure.Media;
using MediaBedrock.Core.Infrastructure.DomainEvents;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Controller.Infrastructure.Bootstrap;

public static class DependencyInjection
{
    public static void AddControllerInfrastructure(this IServiceCollection services)
    {
        services.AddDatabase();

        services.AddDomainEvents();
        services.AddDomainEvent<JobRunStartedDomainEvent, JobRunStartedDomainEventConsumer>();
        services.AddDomainEvent<JobRunStepStartedDomainEvent, JobRunStepStartedDomainEventConsumer>();

        services.AddMessaging(messaging =>
            messaging.AddInMemory(inMemory =>
            {
                inMemory.WithEvent<JobRunFailedIntegrationEvent>();
                inMemory.WithEvent<JobRunStepFailedIntegrationEvent>();
                inMemory.WithEvent<JobRunStepStartedIntegrationEvent>();
                
                inMemory.WithEvent<ProcessorInstanceCompletedIntegrationEvent>();
            }));

        services.AddMedia();
    }
}
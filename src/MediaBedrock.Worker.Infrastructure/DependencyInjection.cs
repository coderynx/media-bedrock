using MediaBedrock.Core.Infrastructure.DomainEvents;
using MediaBedrock.Worker.Application.Processing.DomainEvents;
using MediaBedrock.Worker.Application.Processing.Interfaces;
using MediaBedrock.Worker.Domain.Processing.DomainEvents;
using MediaBedrock.Worker.Infrastructure.Database;
using MediaBedrock.Worker.Infrastructure.Plugins;
using MediaBedrock.Worker.Infrastructure.Processing;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Worker.Infrastructure;

public static class DependencyInjection
{
    public static void AddWorkerInfrastructure(this IServiceCollection services)
    {
        services.AddDomainEvent<ProcessorInstanceStartedDomainEvent, ProcessorInstanceStartedDomainEventConsumer>();
        
        services.AddSingleton<IProcessorProvider, ProcessorProvider>();

        services.AddPlugins();
        services.AddDatabase();
    }
}
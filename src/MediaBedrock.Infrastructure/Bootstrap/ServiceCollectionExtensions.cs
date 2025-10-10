using Coderynx.MessagingKit;
using Coderynx.MessagingKit.Transports.InMemory;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Infrastructure.Database.Bootstrap;
using MediaBedrock.Infrastructure.Media;
using MediaBedrock.Infrastructure.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Infrastructure.Bootstrap;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddDatabase();

        services.AddMessaging(messaging =>
            messaging.AddInMemory(inMemory =>
            {
                inMemory.WithEvent<JobRunCompleted>();
                inMemory.WithEvent<JobRunFailed>();
                inMemory.WithEvent<JobRunStepCompleted>();
                inMemory.WithEvent<JobRunStepFailed>();
                inMemory.WithEvent<ProcessJobRunStep>();
            }));

        services.AddPlugins();
        services.AddMedia();
    }
}
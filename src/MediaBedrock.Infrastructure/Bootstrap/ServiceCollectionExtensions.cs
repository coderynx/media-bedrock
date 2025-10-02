using MediaBedrock.Infrastructure.Database.Bootstrap;
using MediaBedrock.Infrastructure.Media;
using MediaBedrock.Infrastructure.Messaging.Bootstrap;
using MediaBedrock.Infrastructure.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Infrastructure.Bootstrap;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddMessaging();
        services.AddPersistence();

        services.AddPlugins();
        services.AddMedia();
    }
}
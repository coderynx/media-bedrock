using MediaBedrock.Cli.Infrastructure.Media;
using MediaBedrock.Cli.Infrastructure.Messaging.Bootstrap;
using MediaBedrock.Cli.Infrastructure.Persistence.Bootstrap;
using MediaBedrock.Cli.Infrastructure.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Infrastructure.Bootstrap;

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
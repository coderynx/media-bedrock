using MediaBedrock.Cli.Infrastructure.Jobs.Bootstrap;
using MediaBedrock.Cli.Infrastructure.Media;
using MediaBedrock.Cli.Infrastructure.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Infrastructure.Bootstrap;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddMedia();
        services.AddJobs();
        services.AddPlugins();
    }
}
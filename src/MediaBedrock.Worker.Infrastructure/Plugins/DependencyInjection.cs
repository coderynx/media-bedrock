using MediaBedrock.Worker.Infrastructure.Plugins.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Worker.Infrastructure.Plugins;

internal static class DependencyInjection
{
    internal static void AddPlugins(this IServiceCollection services)
    {
        services.AddSingleton<IPluginsManager, PluginsManager>();
        services.AddHostedService<PluginsLoader>();
    }
}
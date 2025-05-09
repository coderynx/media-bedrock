using MediaBedrock.Domain.Processors.Interfaces;
using MediaBedrock.Infrastructure.Plugins.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Infrastructure.Plugins;

internal static class DependencyInjection
{
    internal static void AddPlugins(this IServiceCollection services)
    {
        services.AddSingleton<IProcessorProvider, ProcessorProvider>();
        services.AddSingleton<IPluginsManager, PluginsManager>();
        services.AddHostedService<PluginsLoader>();
    }
}
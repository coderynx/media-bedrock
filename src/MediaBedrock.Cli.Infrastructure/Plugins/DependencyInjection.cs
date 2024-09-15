using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Infrastructure.Plugins.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Infrastructure.Plugins;

internal static class DependencyInjection
{
    internal static void AddPlugins(this IServiceCollection services)
    {
        services.AddSingleton<IProcessorProvider, ProcessorProvider>();
        services.AddSingleton<IPluginsManager, PluginsManager>();
        services.AddHostedService<PluginsLoader>();
    }
}
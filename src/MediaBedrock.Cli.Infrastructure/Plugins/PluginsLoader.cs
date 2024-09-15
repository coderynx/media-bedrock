using MediaBedrock.Cli.Infrastructure.Plugins.Interfaces;
using Microsoft.Extensions.Hosting;

namespace MediaBedrock.Cli.Infrastructure.Plugins;

public sealed class PluginsLoader(IPluginsManager pluginsManager) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        pluginsManager.Initialize();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await pluginsManager.DisposeAsync();
    }
}
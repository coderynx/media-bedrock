using MediaBedrock.Worker.Infrastructure.Plugins.Interfaces;
using Microsoft.Extensions.Hosting;

namespace MediaBedrock.Worker.Infrastructure.Plugins;

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
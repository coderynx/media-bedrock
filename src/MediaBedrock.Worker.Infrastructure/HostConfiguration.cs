using MediaBedrock.Worker.Infrastructure.Database;
using Microsoft.Extensions.Hosting;

namespace MediaBedrock.Worker.Infrastructure;

public static class HostConfiguration
{
    public static void UseWorkerInfrastructure(this IServiceProvider services, IHostEnvironment environment)
    {
        services.UseDatabase(environment);
    }
}
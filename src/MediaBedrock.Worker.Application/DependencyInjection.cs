using MediaBedrock.Worker.Application.Processing;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Worker.Application;

public static class DependencyInjection
{
    public static void AddWorkerApplication(this IServiceCollection services)
    {
        services.AddProcessing();
    }
}
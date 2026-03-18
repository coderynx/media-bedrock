using MediaBedrock.Worker.Application.Processing.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Worker.Application.Processing;

public static class DependencyInjection
{
    internal static void AddProcessing(this IServiceCollection services)
    {
        services.AddScoped<IProcessorRunner, ProcessorRunner>();
        services.AddScoped<IProcessorInstancesService, ProcessorInstancesService>();
        services.AddScoped<IProcessorContextFactory, ProcessorContextFactory>();
    }
}
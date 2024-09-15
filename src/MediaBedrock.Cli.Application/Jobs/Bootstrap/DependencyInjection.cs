using MediaBedrock.Cli.Application.Jobs.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Application.Jobs.Bootstrap;

internal static class DependencyInjection
{
    internal static void AddJobs(this IServiceCollection services)
    {
        services.AddSingleton<IJobFactory, JobFactory>();
        services.AddSingleton<IJobTemplateFactory, JobTemplateFactory>();
        services.AddSingleton<IJobContainerFactory, JobContainerFactory>();
        services.AddSingleton<IJobRunner, JobRunner>();
        services.AddSingleton<IProcessorContextFactory, ProcessorContextFactory>();
    }
}
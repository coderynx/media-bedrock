using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Processors.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Application.Jobs.Bootstrap;

internal static class DependencyInjection
{
    internal static void AddJobs(this IServiceCollection services)
    {
        services.AddScoped<IJobsService, JobsService>();
        services.AddScoped<IJobsStateMachinesService, JobsStateMachinesService>();
        services.AddSingleton<IProcessorContextFactory, ProcessorContextFactory>();
    }
}
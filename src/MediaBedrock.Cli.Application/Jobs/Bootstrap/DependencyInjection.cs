using MediaBedrock.Cli.Application.Jobs.Handlers;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Processors.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Application.Jobs.Bootstrap;

internal static class DependencyInjection
{
    internal static void AddJobs(this IServiceCollection services)
    {
        services.AddSingleton<IJobFactory, JobFactory>();
        services.AddSingleton<IJobStateMachineFactory, JobStateMachineFactory>();
        services.AddScoped<IJobsService, JobsService>();
        services.AddSingleton<IProcessorContextFactory, ProcessorContextFactory>();

        services.AddScoped<IJobMessageHandler<RunJob>, RunJobHandler>();
        services.AddScoped<IJobMessageHandler<ProcessJobStep>, ProcessJobStepHandler>();
    }
}
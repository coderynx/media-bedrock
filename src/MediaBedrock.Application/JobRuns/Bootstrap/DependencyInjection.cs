using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.JobRuns.Services;
using MediaBedrock.Application.Jobs;
using MediaBedrock.Domain.Processors.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Application.JobRuns.Bootstrap;

internal static class DependencyInjection
{
    public static void AddJobRuns(this IServiceCollection services)
    {
        services.AddScoped<IJobRunService, JobRunService>();
        services.AddScoped<IJobRunsOrchestrator, JobRunsOrchestrator>();
        services.AddScoped<IJobRunStepsOrchestrator, JobRunStepsOrchestrator>();
        services.AddSingleton<IProcessorContextFactory, ProcessorContextFactory>();
    }
}
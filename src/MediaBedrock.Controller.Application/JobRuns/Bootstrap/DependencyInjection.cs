using MediaBedrock.Controller.Application.JobRuns.Interfaces;
using MediaBedrock.Controller.Application.JobRuns.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Controller.Application.JobRuns.Bootstrap;

internal static class DependencyInjection
{
    public static void AddJobRuns(this IServiceCollection services)
    {
        services.AddScoped<IJobRunService, JobRunService>();
        services.AddScoped<IJobRunsOrchestrator, JobRunsOrchestrator>();

        services.AddScoped<IJobRunStepsService, JobRunStepsService>();
        services.AddScoped<IJobRunStepsOrchestrator, JobRunStepsOrchestrator>();

        services.AddScoped<IStorageService, StorageService>();
    }
}
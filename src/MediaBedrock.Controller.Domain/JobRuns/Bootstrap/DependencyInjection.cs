using MediaBedrock.Controller.Domain.JobRuns.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Controller.Domain.JobRuns.Bootstrap;

internal static class DependencyInjection
{
    public static void AddJobRuns(this IServiceCollection services)
    {
        services.AddSingleton<IJobRunFactory, JobRunFactory>();
    }
}
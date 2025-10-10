using MediaBedrock.Domain.JobRuns.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Domain.JobRuns.Bootstrap;

internal static class DependencyInjection
{
    public static void AddJobRuns(this IServiceCollection services)
    {
        services.AddSingleton<IJobRunFactory, JobRunFactory>();
    }
}
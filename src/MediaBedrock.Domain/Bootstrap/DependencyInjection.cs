using MediaBedrock.Domain.JobRuns.Bootstrap;
using MediaBedrock.Domain.Jobs.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Domain.Bootstrap;

public static class DependencyInjection
{
    public static void AddDomain(this IServiceCollection services)
    {
        services.AddJobs();
        services.AddJobRuns();
    }
}
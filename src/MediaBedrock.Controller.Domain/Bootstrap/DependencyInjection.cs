using MediaBedrock.Controller.Domain.JobRuns.Bootstrap;
using MediaBedrock.Controller.Domain.Jobs.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Controller.Domain.Bootstrap;

public static class DependencyInjection
{
    public static void AddControllerDomain(this IServiceCollection services)
    {
        services.AddJobs();
        services.AddJobRuns();
    }
}
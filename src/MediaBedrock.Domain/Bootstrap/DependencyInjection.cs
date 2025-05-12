using MediaBedrock.Domain.Jobs.Bootstrap;
using MediaBedrock.Domain.JobStateMachine.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Domain.Bootstrap;

public static class DependencyInjection
{
    public static void AddDomain(this IServiceCollection services)
    {
        services.AddJobs();
        services.AddJobsStateMachines();
    }
}
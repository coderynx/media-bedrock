using MediaBedrock.Cli.Domain.Jobs.Bootstrap;
using MediaBedrock.Cli.Domain.JobsStateMachine.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Domain.Bootstrap;

public static class DependencyInjection
{
    public static void AddDomain(this IServiceCollection services)
    {
        services.AddJobs();
        services.AddJobsStateMachines();
    }
}
using MediaBedrock.Domain.JobStateMachines.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Domain.JobStateMachines.Bootstrap;

internal static class DependencyInjection
{
    public static void AddJobsStateMachines(this IServiceCollection services)
    {
        services.AddSingleton<IJobStateMachineFactory, JobStateMachineFactory>();
    }
}
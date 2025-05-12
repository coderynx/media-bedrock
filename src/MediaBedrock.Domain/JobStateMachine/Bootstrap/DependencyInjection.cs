using MediaBedrock.Domain.JobStateMachine.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Domain.JobStateMachine.Bootstrap;

internal static class DependencyInjection
{
    public static void AddJobsStateMachines(this IServiceCollection services)
    {
        services.AddSingleton<IJobStateMachineFactory, JobStateMachineFactory>();
    }
}
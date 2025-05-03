using MediaBedrock.Cli.Domain.JobsStateMachine.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Domain.JobsStateMachine.Bootstrap;

internal static class DependencyInjection
{
    public static void AddJobsStateMachines(this IServiceCollection services)
    {
        services.AddSingleton<IJobStateMachineFactory, JobStateMachineFactory>();
    }
}
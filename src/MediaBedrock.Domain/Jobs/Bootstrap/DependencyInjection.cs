using MediaBedrock.Domain.Jobs.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Domain.Jobs.Bootstrap;

internal static class DependencyInjection
{
    public static void AddJobs(this IServiceCollection services)
    {
        services.AddSingleton<IJobFactory, JobFactory>();
    }
}
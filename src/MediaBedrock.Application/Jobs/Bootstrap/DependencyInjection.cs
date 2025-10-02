using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Application.Jobs.Bootstrap;

internal static class DependencyInjection
{
    internal static void AddJobs(this IServiceCollection services)
    {
        services.AddScoped<IJobsService, JobsService>();
    }
}
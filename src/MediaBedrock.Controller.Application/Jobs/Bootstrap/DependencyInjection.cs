using MediaBedrock.Controller.Application.Jobs.Interfaces;
using MediaBedrock.Controller.Application.Jobs.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Controller.Application.Jobs.Bootstrap;

internal static class DependencyInjection
{
    internal static void AddJobs(this IServiceCollection services)
    {
        services.AddScoped<IJobsService, JobsService>();
    }
}
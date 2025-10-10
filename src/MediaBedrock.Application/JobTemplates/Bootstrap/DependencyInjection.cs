using MediaBedrock.Application.JobTemplates.Interfaces;
using MediaBedrock.Application.JobTemplates.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Application.JobTemplates.Bootstrap;

public static class DependencyInjection
{
    public static void AddJobTemplates(this IServiceCollection services)
    {
        services.AddScoped<IJobTemplatesService, JobTemplatesService>();
    }
}
using MediaBedrock.Controller.Application.JobTemplates.Interfaces;
using MediaBedrock.Controller.Application.JobTemplates.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Controller.Application.JobTemplates.Bootstrap;

public static class DependencyInjection
{
    public static void AddJobTemplates(this IServiceCollection services)
    {
        services.AddScoped<IJobTemplatesService, JobTemplatesService>();
    }
}
using MediaBedrock.Application.JobTemplates.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Application.JobTemplates.Bootstrap;

public static class DependencyInjection
{
    public static void AddJobTemplates(this IServiceCollection services)
    {
        services.AddScoped<IJobTemplatesService, JobTemplatesService>();
    }
}
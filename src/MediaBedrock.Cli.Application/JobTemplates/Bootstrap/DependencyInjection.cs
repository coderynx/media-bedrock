using MediaBedrock.Cli.Application.JobTemplates.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Application.JobTemplates.Bootstrap;

public static class DependencyInjection
{
    public static void AddJobTemplates(this IServiceCollection services)
    {
        services.AddScoped<IJobTemplatesService, JobTemplatesService>();
    }
}
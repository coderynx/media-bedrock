using MediaBedrock.Cli.Domain.JobTemplates.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Infrastructure.JobTemplates.Bootstrap;

internal static class DependencyInjection
{
    public static void AddJobTemplates(this IServiceCollection services)
    {
        services.AddSingleton<IJobTemplateSerializer, IJobTemplateSerializer>();
    }
}
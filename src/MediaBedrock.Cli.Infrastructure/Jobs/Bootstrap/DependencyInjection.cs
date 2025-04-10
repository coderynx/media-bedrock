using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Infrastructure.Jobs.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Bootstrap;

internal static class DependencyInjection
{
    internal static void AddJobs(this IServiceCollection services)
    {
        services.AddSingleton<IJobSerializer, JobJsonSerializer>();
        services.AddSingleton<IJobTemplateSerializer, JobTemplateJsonSerializer>();
        services.AddSingleton<IBatchJobSerializer, BatchJobJsonSerializer>();
    }
}
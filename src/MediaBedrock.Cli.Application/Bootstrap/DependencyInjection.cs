using MediaBedrock.Cli.Application.Jobs.Bootstrap;
using MediaBedrock.Cli.Application.JobTemplates.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Application.Bootstrap;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddJobs();
        services.AddJobTemplates();
    }
}
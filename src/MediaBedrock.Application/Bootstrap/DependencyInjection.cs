using MediaBedrock.Application.Jobs.Bootstrap;
using MediaBedrock.Application.JobTemplates.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Application.Bootstrap;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddJobs();
        services.AddJobTemplates();
    }
}
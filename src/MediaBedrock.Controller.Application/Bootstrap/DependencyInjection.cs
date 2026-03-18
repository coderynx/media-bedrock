using MediaBedrock.Controller.Application.JobRuns.Bootstrap;
using MediaBedrock.Controller.Application.Jobs.Bootstrap;
using MediaBedrock.Controller.Application.JobTemplates.Bootstrap;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Controller.Application.Bootstrap;

public static class DependencyInjection
{
    public static void AddControllerApplication(this IServiceCollection services)
    {
        services.AddJobTemplates();
        services.AddJobs();
        services.AddJobRuns();
    }
}
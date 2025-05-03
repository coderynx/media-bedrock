using MediaBedrock.Cli.Domain.BatchJobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Infrastructure.Jobs.Batch;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Bootstrap;

internal static class DependencyInjection
{
    internal static void AddJobs(this IServiceCollection services)
    {
        services.AddSingleton<IJobSerializer, JobJsonSerializer>();
        services.AddSingleton<IBatchJobSerializer, BatchJobJsonSerializer>();
    }
}
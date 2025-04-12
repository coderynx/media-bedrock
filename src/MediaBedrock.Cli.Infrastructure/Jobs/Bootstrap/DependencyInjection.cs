using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Infrastructure.Jobs.Batch;
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

        services.AddScoped<IJobTemplatesRepository, JobTemplatesFileSystemRepository>();
        services.AddSingleton<IJobStateMachineRepository, InMemoryJobStateMachineRepository>();

        services.AddSingleton<JobMessageQueue>();
        services.AddSingleton<IJobMessageBus, JobMessageBus>();
        services.AddHostedService<JobEventProcessorBackgroundService>();
    }
}
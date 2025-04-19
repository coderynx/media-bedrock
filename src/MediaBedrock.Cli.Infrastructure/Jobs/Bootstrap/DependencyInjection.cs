using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Templates.Interfaces;
using MediaBedrock.Cli.Infrastructure.Jobs.Batch;
using MediaBedrock.Cli.Infrastructure.Jobs.Templates;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Bootstrap;

internal static class DependencyInjection
{
    internal static void AddJobs(this IServiceCollection services)
    {
        services.AddSingleton<IJobTemplateSerializerProvider, JobTemplateSerializerProvider>();
        services.AddSingleton<JobTemplateYamlSerializer>();
        services.AddSingleton<JobTemplateJsonSerializer>();

        services.AddSingleton<IJobSerializerProvider, JobSerializerProvider>();
        services.AddSingleton<JobYamlSerializer>();
        services.AddSingleton<JobJsonSerializer>();

        services.AddSingleton<IJobTemplateSerializer, JobTemplateJsonSerializer>();
        services.AddSingleton<IBatchJobSerializer, BatchJobJsonSerializer>();

        services.AddScoped<IJobTemplatesRepository, JobTemplatesFileSystemRepository>();
        services.AddSingleton<IJobStateMachineRepository, InMemoryJobStateMachineRepository>();

        services.AddSingleton<JobMessageQueue>();
        services.AddSingleton<IJobMessageBus, JobMessageBus>();
        services.AddHostedService<JobEventProcessorBackgroundService>();
    }
}
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Templates;
using MediaBedrock.Cli.Domain.Jobs.Templates.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

public sealed class JobTemplateSerializerProvider(IServiceProvider serviceProvider) : IJobTemplateSerializerProvider
{
    public Result<IJobTemplateSerializer> ResolveSerializer(JobTemplateSerializerFormat format)
    {
        IJobTemplateSerializer? serializer = format switch
        {
            JobTemplateSerializerFormat.Json => serviceProvider.GetRequiredService<JobTemplateJsonSerializer>(),
            JobTemplateSerializerFormat.Yaml => serviceProvider.GetRequiredService<JobTemplateYamlSerializer>(),
            _ => null
        };

        return serializer is null
            ? JobTemplateErrors.InvalidSerializerFormat()
            : Result.Created(serializer);
    }

    public Result<IJobTemplateSerializer> ResolveSerializer(string extension)
    {
        JobTemplateSerializerFormat? serializerFormat = extension switch
        {
            ".json" => JobTemplateSerializerFormat.Json,
            ".yaml" => JobTemplateSerializerFormat.Yaml,
            ".yml" => JobTemplateSerializerFormat.Yaml,
            _ => null
        };

        return serializerFormat is null
            ? JobTemplateErrors.InvalidSerializerFormat()
            : ResolveSerializer(serializerFormat.Value);
    }
}
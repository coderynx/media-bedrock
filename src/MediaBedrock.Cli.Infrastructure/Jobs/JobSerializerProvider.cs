using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

/// <inheritdoc />
public sealed class JobSerializerProvider(IServiceProvider serviceProvider) : IJobSerializerProvider
{
    /// <inheritdoc />
    public Result<IJobSerializer> ResolveSerializer(JobSerializerFormat format)
    {
        IJobSerializer? serializer = format switch
        {
            JobSerializerFormat.Json => serviceProvider.GetRequiredService<JobJsonSerializer>(),
            JobSerializerFormat.Yaml => serviceProvider.GetRequiredService<JobYamlSerializer>(),
            _ => null
        };

        return serializer is null
            ? JobErrors.InvalidSerializerFormat()
            : Result.Created(serializer);
    }

    /// <inheritdoc />
    public Result<IJobSerializer> ResolveSerializer(string extension)
    {
        JobSerializerFormat? serializerFormat = extension switch
        {
            ".json" => JobSerializerFormat.Json,
            ".yaml" => JobSerializerFormat.Yaml,
            ".yml" => JobSerializerFormat.Yaml,
            _ => null
        };

        return serializerFormat is null
            ? JobErrors.InvalidSerializerFormat()
            : ResolveSerializer(serializerFormat.Value);
    }
}
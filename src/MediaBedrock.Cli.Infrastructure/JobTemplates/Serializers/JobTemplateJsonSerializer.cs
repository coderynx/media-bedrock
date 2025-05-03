using System.Text.Json;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Domain.JobTemplates.Interfaces;
using MediaBedrock.Cli.Infrastructure.Jobs;

namespace MediaBedrock.Cli.Infrastructure.JobTemplates.Serializers;

/// <inheritdoc />
public sealed class JobTemplateJsonSerializer : IJobTemplateSerializer
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters =
        {
            new JobTemplateNameJsonConverter(),
            new JobTemplateVersionJsonConverter(),
            new JobTemplateAuthorJsonConverter(),
            new ProcessorNameJsonConverter()
        },
        WriteIndented = true
    };

    /// <inheritdoc />
    public Result<string> Serialize(JobTemplate jobTemplate)
    {
        try
        {
            var json = JsonSerializer.Serialize(jobTemplate, _serializerOptions);
            return Result.Created(json);
        }
        catch (Exception e)
        {
            return JobTemplateErrors.ManifestSerializationFailed(e.Message);
        }
    }

    /// <inheritdoc />
    public Result<JobTemplate> Deserialize(string serialized)
    {
        var jobTemplate = JsonSerializer.Deserialize<JobTemplate>(serialized, _serializerOptions);

        return jobTemplate is null
            ? JobTemplateErrors.ManifestDeserializationFailed(serialized)
            : Result.Created(jobTemplate);
    }
}
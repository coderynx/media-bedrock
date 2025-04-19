using System.Text.Json;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Templates;
using MediaBedrock.Cli.Domain.Jobs.Templates.Interfaces;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

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
            return JobTemplateErrors.SerializationFailed(e.Message);
        }
    }

    /// <inheritdoc />
    public Result<JobTemplate> Deserialize(string serialized)
    {
        var jobTemplate = JsonSerializer.Deserialize<JobTemplate>(serialized, _serializerOptions);

        return jobTemplate is null
            ? JobTemplateErrors.DeserializationFailed(serialized)
            : Result.Created(jobTemplate);
    }
}
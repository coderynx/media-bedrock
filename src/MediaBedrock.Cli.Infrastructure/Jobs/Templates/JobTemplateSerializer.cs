using System.Text.Json;
using Coderynx.Functional.Result;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Templates;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

public sealed class JobTemplateSerializer : IJobTemplateSerializer
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new JobTemplateNameConverter() }
    };

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

    public Result<JobTemplate> Deserialize(string serialized)
    {
        var jobTemplate = JsonSerializer.Deserialize<JobTemplate>(serialized, _serializerOptions);

        return jobTemplate is null
            ? JobTemplateErrors.DeserializationFailed(serialized)
            : Result.Created(jobTemplate);
    }
}
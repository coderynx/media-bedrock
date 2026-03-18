using System.Text.Json;
using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.BatchJobs;
using MediaBedrock.Controller.Domain.BatchJobs.Interfaces;
using MediaBedrock.Controller.Domain.JobTemplates;
using MediaBedrock.Controller.Infrastructure.JobTemplates.Serializers;

namespace MediaBedrock.Controller.Infrastructure.Jobs.Batch;

public sealed class BatchJobParametersJsonSerializer : IBatchJobParametersSerializer
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new JobTemplateNameJsonConverter() }
    };

    public Result<string> Serialize(BatchJobParameters parameters)
    {
        try
        {
            var json = JsonSerializer.Serialize(parameters, _serializerOptions);
            return Result.Created(json);
        }
        catch (Exception e)
        {
            return JobTemplateErrors.ManifestSerializationFailed(e.Message);
        }
    }

    public Result<BatchJobParameters> Deserialize(string serialized)
    {
        var jobTemplate = JsonSerializer.Deserialize<BatchJobParameters>(serialized, _serializerOptions);

        return jobTemplate is null
            ? JobTemplateErrors.ManifestDeserializationFailed(serialized)
            : Result.Created(jobTemplate);
    }
}
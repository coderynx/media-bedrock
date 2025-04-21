using System.Text.Json;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Batches;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Infrastructure.JobTemplates.Serializers;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Batch;

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
            return JobTemplateErrors.SerializationFailed(e.Message);
        }
    }

    public Result<BatchJobParameters> Deserialize(string serialized)
    {
        var jobTemplate = JsonSerializer.Deserialize<BatchJobParameters>(serialized, _serializerOptions);

        return jobTemplate is null
            ? JobTemplateErrors.DeserializationFailed(serialized)
            : Result.Created(jobTemplate);
    }
}
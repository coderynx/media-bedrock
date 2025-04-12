using System.Text.Json;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Batches;
using MediaBedrock.Cli.Domain.Jobs.Templates;
using MediaBedrock.Cli.Infrastructure.Jobs.Templates;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Batch;

public sealed class BatchJobParametersJsonSerializer : IBatchJobParametersSerializer
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters = { new JobTemplateNameConverter() }
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
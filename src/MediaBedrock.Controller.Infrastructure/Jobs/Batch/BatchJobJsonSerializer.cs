using System.Text.Json;
using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.BatchJobs;
using MediaBedrock.Controller.Domain.BatchJobs.Interfaces;
using MediaBedrock.Controller.Domain.Jobs;
using MediaBedrock.Controller.Infrastructure.JobTemplates.Serializers;

namespace MediaBedrock.Controller.Infrastructure.Jobs.Batch;

/// <summary>
///     Provides functionality to serialize and deserialize <see cref="BatchJob" /> objects to and from JSON format.
///     Implements the <see cref="IBatchJobSerializer" /> interface.
/// </summary>
public sealed class BatchJobJsonSerializer : IBatchJobSerializer
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters =
        {
            new BatchJobIdConverter(),
            new JobIdJsonConverter(),
            new JobTemplateNameJsonConverter(),
            new JobStepNameJsonConverter()
        }
    };

    /// <inheritdoc />
    public Result<string> Serialize(BatchJob batchJob)
    {
        try
        {
            var json = JsonSerializer.Serialize(batchJob, _serializerOptions);
            return Result.Created(json);
        }
        catch (Exception e)
        {
            return JobErrors.SerializationFailed(e.Message);
        }
    }

    /// <inheritdoc />
    public Result<BatchJob> Deserialize(string serialized)
    {
        var job = JsonSerializer.Deserialize<BatchJob>(serialized, _serializerOptions);

        return job is null
            ? BatchJobErrors.DeserializationFailed(serialized)
            : Result.Created(job);
    }
}
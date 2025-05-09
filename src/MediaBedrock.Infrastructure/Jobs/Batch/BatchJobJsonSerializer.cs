using System.Text.Json;
using Coderynx.Functional.Results;
using MediaBedrock.Domain.BatchJobs;
using MediaBedrock.Domain.BatchJobs.Interfaces;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Infrastructure.JobTemplates.Serializers;

namespace MediaBedrock.Infrastructure.Jobs.Batch;

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
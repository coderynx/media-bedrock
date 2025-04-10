using System.Text.Json;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Infrastructure.Jobs.Templates;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

/// <summary>
///     Provides functionality to serialize and deserialize <see cref="Job" /> objects to and from JSON format.
///     Implements the <see cref="IJobSerializer" /> interface.
/// </summary>
public sealed class JobJsonSerializer : IJobSerializer
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters =
        {
            new JobIdConverter(),
            new JobTemplateNameConverter(),
            new JobStepNameConverter()
        },
        WriteIndented = true
    };

    /// <inheritdoc />
    public Result<string> Serialize(Job job)
    {
        try
        {
            var json = JsonSerializer.Serialize(job, _serializerOptions);
            return Result.Created(json);
        }
        catch (Exception e)
        {
            return JobErrors.SerializationFailed(e.Message);
        }
    }

    /// <inheritdoc />
    public Result<Job> Deserialize(string serialized)
    {
        var job = JsonSerializer.Deserialize<Job>(serialized, _serializerOptions);

        return job is null
            ? JobErrors.DeserializationFailed(serialized)
            : Result.Created(job);
    }
}
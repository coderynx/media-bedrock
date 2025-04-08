using System.Text.Json;
using Coderynx.Functional.Result;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Infrastructure.Jobs.Templates;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

public sealed class JobSerializer : IJobSerializer
{
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        Converters =
        {
            new JobIdConverter(),
            new JobTemplateNameConverter(),
            new JobStepNameConverter()
        }
    };

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

    public Result<Job> Deserialize(string serialized)
    {
        var job = JsonSerializer.Deserialize<Job>(serialized, _serializerOptions);

        return job is null
            ? JobErrors.DeserializationFailed(serialized)
            : Result.Created(job);
    }
}
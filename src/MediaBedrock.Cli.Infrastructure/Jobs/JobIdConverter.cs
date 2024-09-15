using System.Text.Json;
using System.Text.Json.Serialization;
using MediaBedrock.Cli.Domain.Jobs;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

public sealed class JobIdConverter : JsonConverter<JobId>
{
    public override JobId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetGuid();
        
        var jobId = JobId.Create(value);
        if (jobId.IsFailure)
        {
            throw new JsonException(jobId.Error.Message);
        }
        
        return jobId.Value;
    }

    public override void Write(Utf8JsonWriter writer, JobId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
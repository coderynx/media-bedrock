using System.Text.Json;
using System.Text.Json.Serialization;
using MediaBedrock.Controller.Domain.Jobs;

namespace MediaBedrock.Controller.Infrastructure.Jobs;

public sealed class JobIdJsonConverter : JsonConverter<JobId>
{
    public override JobId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetGuid();

        var jobId = new JobId(value);
        return jobId;
    }

    public override void Write(Utf8JsonWriter writer, JobId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
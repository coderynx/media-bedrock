using System.Text.Json;
using System.Text.Json.Serialization;
using MediaBedrock.Cli.Domain.Jobs.Batches;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

public sealed class BatchJobIdConverter : JsonConverter<BatchJobId>
{
    public override BatchJobId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetGuid();

        var jobId = BatchJobId.Create(value);
        if (jobId.IsFailure)
        {
            throw new JsonException(jobId.Error.Message);
        }

        return jobId.Value;
    }

    public override void Write(Utf8JsonWriter writer, BatchJobId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
using System.Text.Json;
using System.Text.Json.Serialization;
using MediaBedrock.Cli.Domain.Jobs.Processors;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

public sealed class ProcessorNameJsonConverter : JsonConverter<ProcessorName>
{
    public override ProcessorName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is null)
        {
            throw new JsonException("Job step name cannot be null or empty.");
        }

        var jobId = ProcessorName.Create(value);
        if (jobId.IsFailure)
        {
            throw new JsonException(jobId.Error.Message);
        }

        return jobId.Value;
    }

    public override void Write(Utf8JsonWriter writer, ProcessorName value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
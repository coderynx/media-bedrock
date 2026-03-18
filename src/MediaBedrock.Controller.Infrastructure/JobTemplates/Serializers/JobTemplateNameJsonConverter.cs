using System.Text.Json;
using System.Text.Json.Serialization;
using MediaBedrock.Controller.Domain.JobTemplates;

namespace MediaBedrock.Controller.Infrastructure.JobTemplates.Serializers;

public sealed class JobTemplateNameJsonConverter : JsonConverter<JobTemplateName>
{
    public override JobTemplateName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is null)
        {
            throw new JsonException($"{nameof(JobTemplateName)} cannot be null");
        }

        return new JobTemplateName(value);
    }

    public override void Write(Utf8JsonWriter writer, JobTemplateName value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
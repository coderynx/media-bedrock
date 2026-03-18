using System.Text.Json;
using System.Text.Json.Serialization;
using MediaBedrock.Controller.Domain.JobTemplates;

namespace MediaBedrock.Controller.Infrastructure.JobTemplates.Serializers;

public sealed class JobTemplateVersionJsonConverter : JsonConverter<JobTemplateVersion>
{
    public override JobTemplateVersion Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is null)
        {
            throw new JsonException($"{nameof(JobTemplateVersion)} cannot be null");
        }

        var createJobTemplateVersion = JobTemplateVersion.Create(value);
        if (createJobTemplateVersion.IsFailure)
        {
            throw createJobTemplateVersion.Error;
        }

        return createJobTemplateVersion.Value;
    }

    public override void Write(Utf8JsonWriter writer, JobTemplateVersion value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
using System.Text.Json;
using System.Text.Json.Serialization;
using MediaBedrock.Cli.Domain.JobTemplates;

namespace MediaBedrock.Cli.Infrastructure.JobTemplates.Serializers;

public sealed class JobTemplateAuthorJsonConverter : JsonConverter<JobTemplateAuthor>
{
    public override JobTemplateAuthor Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is null)
        {
            throw new JsonException($"{nameof(JobTemplateAuthor)} cannot be null");
        }

        var createJobTemplateAuthor = JobTemplateAuthor.Create(value);
        if (createJobTemplateAuthor.IsFailure)
        {
            throw new JsonException(createJobTemplateAuthor.Error.Message);
        }

        return createJobTemplateAuthor.Value;
    }

    public override void Write(Utf8JsonWriter writer, JobTemplateAuthor value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
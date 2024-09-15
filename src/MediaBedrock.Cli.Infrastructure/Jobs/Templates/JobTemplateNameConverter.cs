using System.Text.Json;
using System.Text.Json.Serialization;
using MediaBedrock.Cli.Domain.Jobs.Templates;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

public sealed class JobTemplateNameConverter : JsonConverter<JobTemplateName>
{
    public override JobTemplateName Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is null)
        {
            throw new JsonException($"{nameof(JobTemplateName)} cannot be null");
        }

        var createJobTemplate = JobTemplateName.Create(value);
        if (createJobTemplate.IsFailure)
        {
            throw new JsonException(createJobTemplate.Error.Message);
        }

        return createJobTemplate.Value;
    }

    public override void Write(Utf8JsonWriter writer, JobTemplateName value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}
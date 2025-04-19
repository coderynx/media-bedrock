using MediaBedrock.Cli.Domain.Jobs.Templates;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

public sealed class JobTemplateAuthorYamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(JobTemplateAuthor);
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var value = parser.Consume<Scalar>().Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new YamlException("Job template author cannot be null or empty.");
        }

        var createJobTemplateAuthor = JobTemplateAuthor.Create(value);
        if (createJobTemplateAuthor.IsFailure)
        {
            throw new YamlException(createJobTemplateAuthor.Error.Message);
        }

        return createJobTemplateAuthor.Value;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is not JobTemplateAuthor jobTemplateAuthor)
        {
            throw new YamlException($"Expected {nameof(JobTemplateAuthor)} but got {value?.GetType().Name}");
        }

        emitter.Emit(new Scalar(jobTemplateAuthor.Value));
    }
}
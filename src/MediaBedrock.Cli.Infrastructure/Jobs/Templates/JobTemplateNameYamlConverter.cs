using MediaBedrock.Cli.Domain.Jobs.Templates;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

public sealed class JobTemplateNameYamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(JobTemplateName);
    }

    public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        if (parser.Current is not Scalar scalar)
        {
            throw new YamlException(message: $"{nameof(JobTemplateName)} must be a scalar value.");
        }

        var createJobTemplate = JobTemplateName.Create(scalar.Value);
        if (createJobTemplate.IsFailure)
        {
            throw new YamlException(message: createJobTemplate.Error.Message);
        }

        parser.MoveNext();
        return createJobTemplate.Value;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is not JobTemplateName jobTemplateName)
        {
            throw new ArgumentException($"Expected {nameof(JobTemplateName)} but got {value?.GetType().Name}.",
                nameof(value));
        }

        emitter.Emit(new Scalar(jobTemplateName.Value));
    }
}
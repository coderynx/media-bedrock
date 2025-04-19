using MediaBedrock.Cli.Domain.Jobs.Templates;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

public sealed class JobTemplateVersionYamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(JobTemplateVersion);
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var value = parser.Consume<Scalar>().Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new YamlException("Job template version cannot be null or empty.");
        }

        var createJobTemplateVersion = JobTemplateVersion.Create(value);
        if (createJobTemplateVersion.IsFailure)
        {
            throw new YamlException(createJobTemplateVersion.Error.Message);
        }

        return createJobTemplateVersion.Value;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is not JobTemplateVersion jobTemplateVersion)
        {
            throw new YamlException($"Expected {nameof(JobTemplateVersion)} but got {value?.GetType().Name}");
        }

        emitter.Emit(new Scalar(jobTemplateVersion.Value));
    }
}
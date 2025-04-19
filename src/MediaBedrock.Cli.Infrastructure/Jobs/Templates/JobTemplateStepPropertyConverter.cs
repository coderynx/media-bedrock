using MediaBedrock.Cli.Domain.Jobs.Templates;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

public class JobTemplateStepPropertyConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(List<JobTemplateStepProperty>);
    }

    public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var result = new List<JobTemplateStepProperty>();
        parser.Consume<MappingStart>();

        while (!parser.TryConsume<MappingEnd>(out _))
        {
            var key = parser.Consume<Scalar>().Value;
            var value = parser.Consume<Scalar>().Value;

            result.Add(new JobTemplateStepProperty { Name = key, Value = value });
        }

        return result;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        var list = (List<JobTemplateStepProperty>?)value;

        emitter.Emit(new MappingStart());
        foreach (var kv in list ?? Enumerable.Empty<JobTemplateStepProperty>())
        {
            emitter.Emit(new Scalar(kv.Name));
            emitter.Emit(new Scalar(kv.Value));
        }

        emitter.Emit(new MappingEnd());
    }
}
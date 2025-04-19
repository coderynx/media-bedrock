using MediaBedrock.Cli.Domain.Jobs.Templates;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

public sealed class JobTemplateStepSourceYamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(List<JobTemplateStepSource>);
    }

    public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var result = new List<JobTemplateStepSource>();
        parser.Consume<MappingStart>();

        while (!parser.TryConsume<MappingEnd>(out _))
        {
            var name = parser.Consume<Scalar>().Value;
            var destination = parser.Consume<Scalar>().Value;

            result.Add(new JobTemplateStepSource { Name = name, Destination = destination });
        }

        return result;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        var list = (List<JobTemplateStepSource>?)value;

        emitter.Emit(new MappingStart());
        foreach (var kv in list ?? Enumerable.Empty<JobTemplateStepSource>())
        {
            emitter.Emit(new Scalar(kv.Name));
            emitter.Emit(new Scalar(kv.Destination));
        }

        emitter.Emit(new MappingEnd());
    }
}
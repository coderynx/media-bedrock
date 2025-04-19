using MediaBedrock.Cli.Domain.Jobs.Templates;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

public sealed class JobTemplateStepSinkYamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(List<JobTemplateStepSink>);
    }

    public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var result = new List<JobTemplateStepSink>();
        parser.Consume<MappingStart>();

        while (!parser.TryConsume<MappingEnd>(out _))
        {
            var name = parser.Consume<Scalar>().Value;
            var source = parser.Consume<Scalar>().Value;

            result.Add(new JobTemplateStepSink { Name = name, Source = source });
        }

        return result;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        var list = (List<JobTemplateStepSink>?)value;

        emitter.Emit(new MappingStart());
        foreach (var kv in list ?? Enumerable.Empty<JobTemplateStepSink>())
        {
            emitter.Emit(new Scalar(kv.Name));
            emitter.Emit(new Scalar(kv.Source));
        }

        emitter.Emit(new MappingEnd());
    }
}
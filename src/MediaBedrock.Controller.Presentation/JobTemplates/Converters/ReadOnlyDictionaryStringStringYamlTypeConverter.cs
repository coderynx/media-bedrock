using System.Collections.ObjectModel;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MediaBedrock.Controller.Presentation.JobTemplates.Converters;

public sealed class ReadOnlyDictionaryStringStringYamlTypeConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type.IsGenericType &&
               type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>) &&
               type.GetGenericArguments()[0] == typeof(string) &&
               type.GetGenericArguments()[1] == typeof(string);
    }

    public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var dict = new Dictionary<string, string>();
        parser.Consume<MappingStart>();
        while (!parser.TryConsume<MappingEnd>(out _))
        {
            var key = parser.Consume<Scalar>().Value;
            var value = parser.Consume<Scalar>().Value;
            dict[key] = value;
        }

        return new ReadOnlyDictionary<string, string>(dict);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        var dict = (IReadOnlyDictionary<string, string>)value!;
        emitter.Emit(new MappingStart());
        foreach (var kvp in dict)
        {
            emitter.Emit(new Scalar(kvp.Key));
            emitter.Emit(new Scalar(kvp.Value));
        }

        emitter.Emit(new MappingEnd());
    }
}
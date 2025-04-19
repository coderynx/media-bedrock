using MediaBedrock.Cli.Domain.Jobs.Processors;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

public class ProcessorNameYamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(ProcessorName);
    }

    public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        if (parser.Current is not Scalar scalar)
        {
            throw new YamlException(message: $"{nameof(ProcessorName)} must be a scalar value.");
        }

        var createProcessorName = ProcessorName.Create(scalar.Value);
        if (createProcessorName.IsFailure)
        {
            throw new YamlException(message: createProcessorName.Error.Message);
        }

        parser.MoveNext();
        return createProcessorName.Value;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is not ProcessorName processorName)
        {
            throw new ArgumentException(
                message: $"Expected {nameof(ProcessorName)} but got {value?.GetType().Name}.",
                paramName: nameof(value));
        }

        emitter.Emit(new Scalar(processorName.ToString()));
    }
}
using MediaBedrock.Cli.Domain.Jobs;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

public sealed class JobIdYamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(JobId);
    }

    public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var scalar = parser.Consume<Scalar>();
        if (!Guid.TryParse(scalar.Value, out var guid))
        {
            throw new YamlException(scalar.Start, scalar.End, "Invalid GUID format.");
        }

        var jobId = JobId.Create(guid);
        if (jobId.IsSuccess)
        {
            return jobId.Value;
        }

        throw new YamlException(scalar.Start, scalar.End, jobId.Error.Message);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is JobId jobId)
        {
            emitter.Emit(new Scalar(jobId.Value.ToString()));
        }
        else
        {
            throw new InvalidOperationException("Invalid type for JobIdYamlConverter.");
        }
    }
}
using MediaBedrock.Cli.Domain.Jobs.Steps;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

public sealed class JobStepNameYamlConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(JobStepName);
    }

    public object ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var value = parser.Consume<Scalar>().Value;
        if (string.IsNullOrEmpty(value))
        {
            throw new YamlException("Job step name cannot be null or empty.");
        }

        var jobId = JobStepName.Create(value);
        if (jobId.IsFailure)
        {
            throw new YamlException(jobId.Error.Message);
        }

        return jobId.Value;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is not JobStepName jobStepName)
        {
            throw new ArgumentException("Expected a JobStepName object.", nameof(value));
        }

        emitter.Emit(new Scalar(jobStepName.Value));
    }
}
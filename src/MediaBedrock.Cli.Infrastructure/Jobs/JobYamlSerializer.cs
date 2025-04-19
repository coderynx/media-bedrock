using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Infrastructure.Jobs.Templates;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

/// <inheritdoc />
public sealed class JobYamlSerializer : IJobSerializer
{
    private readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithTypeConverter(new JobIdYamlConverter())
        .WithTypeConverter(new JobTemplateNameYamlConverter())
        .WithTypeConverter(new JobStepNameYamlConverter())
        .WithTypeConverter(new ProcessorNameYamlConverter())
        .IgnoreUnmatchedProperties()
        .Build();

    private readonly ISerializer _serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .WithTypeConverter(new JobIdYamlConverter())
        .WithTypeConverter(new JobTemplateNameYamlConverter())
        .WithTypeConverter(new JobStepNameYamlConverter())
        .WithTypeConverter(new ProcessorNameYamlConverter())
        .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
        .Build();

    /// <inheritdoc />
    public Result<string> Serialize(Job job)
    {
        try
        {
            var serialized = _serializer.Serialize(job);
            return Result.Created(serialized);
        }
        catch (Exception e)
        {
            return JobErrors.SerializationFailed(e.Message);
        }
    }

    /// <inheritdoc />
    public Result<Job> Deserialize(string serialized)
    {
        try
        {
            var job = _deserializer.Deserialize<Job>(serialized);
            return Result.Created(job);
        }
        catch (Exception e)
        {
            return JobErrors.DeserializationFailed(e.Message);
        }
    }
}
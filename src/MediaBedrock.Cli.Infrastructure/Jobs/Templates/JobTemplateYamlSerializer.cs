using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Templates;
using MediaBedrock.Cli.Domain.Jobs.Templates.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Templates;

public sealed class JobTemplateYamlSerializer : IJobTemplateSerializer
{
    private readonly IDeserializer _deserializer = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .WithTypeConverter(new JobTemplateNameYamlConverter())
        .WithTypeConverter(new JobTemplateVersionYamlConverter())
        .WithTypeConverter(new JobTemplateAuthorYamlConverter())
        .WithTypeConverter(new JobTemplateStepSinkYamlConverter())
        .WithTypeConverter(new JobTemplateStepSourceYamlConverter())
        .WithTypeConverter(new JobTemplateStepPropertyConverter())
        .WithTypeConverter(new ProcessorNameYamlConverter())
        .Build();

    private readonly ISerializer _serializer = new SerializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .WithTypeConverter(new JobTemplateNameYamlConverter())
        .WithTypeConverter(new JobTemplateVersionYamlConverter())
        .WithTypeConverter(new JobTemplateAuthorYamlConverter())
        .WithTypeConverter(new JobTemplateStepSinkYamlConverter())
        .WithTypeConverter(new JobTemplateStepSourceYamlConverter())
        .WithTypeConverter(new JobTemplateStepPropertyConverter())
        .WithTypeConverter(new ProcessorNameYamlConverter())
        .Build();

    public Result<string> Serialize(JobTemplate jobTemplate)
    {
        try
        {
            var serialized = _serializer.Serialize(jobTemplate);
            return Result.Created(serialized);
        }
        catch (Exception e)
        {
            return JobTemplateErrors.SerializationFailed(e.Message);
        }
    }

    public Result<JobTemplate> Deserialize(string serialized)
    {
        try
        {
            var jobTemplate = _deserializer.Deserialize<JobTemplate>(serialized);
            return Result.Created(jobTemplate);
        }
        catch (Exception e)
        {
            return JobTemplateErrors.DeserializationFailed(e.Message);
        }
    }
}
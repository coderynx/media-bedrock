using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Templates.Interfaces;

public interface IJobTemplateSerializer
{
    Result<string> Serialize(JobTemplate jobTemplate);
    Result<JobTemplate> Deserialize(string serialized);
}
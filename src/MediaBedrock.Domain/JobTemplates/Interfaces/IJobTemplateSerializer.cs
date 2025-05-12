using Coderynx.Functional.Results;

namespace MediaBedrock.Domain.JobTemplates.Interfaces;

public interface IJobTemplateSerializer
{
    Result<string> Serialize(JobTemplate jobTemplate);
    Result<JobTemplate> Deserialize(string serialized);
}
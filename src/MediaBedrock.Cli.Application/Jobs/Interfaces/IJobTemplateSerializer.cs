using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Templates;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IJobTemplateSerializer
{
    Result<string> Serialize(JobTemplate jobTemplate);
    Result<JobTemplate> Deserialize(string serialized);
}
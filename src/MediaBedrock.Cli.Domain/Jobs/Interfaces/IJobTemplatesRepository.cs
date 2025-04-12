using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Templates;

namespace MediaBedrock.Cli.Domain.Jobs.Interfaces;

public interface IJobTemplatesRepository
{
    Task<Result> StoreAsync(JobTemplate jobTemplate);
    Task<Option<JobTemplate>> GetAsync(JobTemplateName name);
    Task<Result> DeleteAsync(JobTemplateName name);
}
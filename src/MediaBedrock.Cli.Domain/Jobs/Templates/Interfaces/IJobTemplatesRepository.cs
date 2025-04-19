using Coderynx.Functional.Options;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Templates.Interfaces;

public interface IJobTemplatesRepository
{
    Task<Result> StoreAsync(JobTemplate jobTemplate);
    Task<Option<JobTemplate>> GetAsync(JobTemplateName name);
    Task<Result> DeleteAsync(JobTemplateName name);
}
using Coderynx.Functional.Results;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Parameters;
using MediaBedrock.Domain.JobTemplates;

namespace MediaBedrock.Application.Jobs.Interfaces;

public interface IJobsService
{
    Task<Result<Job>> CreateAsync(
        JobTemplateName jobTemplateName,
        JobParameters parameters,
        CancellationToken cancellationToken = new());
}
using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.Jobs;
using MediaBedrock.Controller.Domain.Jobs.Parameters;
using MediaBedrock.Controller.Domain.JobTemplates;

namespace MediaBedrock.Controller.Application.Jobs.Interfaces;

public interface IJobsService
{
    Task<Result<Job>> CreateAsync(
        JobTemplateName jobTemplateName,
        JobParameters parameters,
        CancellationToken cancellationToken = new());
}
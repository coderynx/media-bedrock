using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.JobTemplates;

namespace MediaBedrock.Application.JobRuns.Interfaces;

public interface IJobRunService
{
    Task<Result<JobRunId>> CreateAsync(JobId jobId, CancellationToken cancellationToken = new());

    Task<List<JobRun>> GetAsync(JobTemplateName jobTemplateName, CancellationToken ct = default);

    Task DeleteAsync(
        JobTemplateName jobTemplateName,
        JobRunStatus jobRunStatus = JobRunStatus.Pending,
        CancellationToken cancellationToken = default);
}
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.JobTemplates;

namespace MediaBedrock.Application.JobRuns.Interfaces;

public interface IJobRunService
{
    Task<List<JobRun>> GetAsync(JobTemplateName jobTemplateName, CancellationToken ct = default);

    Task DeleteAsync(
        JobTemplateName jobTemplateName,
        JobRunStatus jobRunStatus = JobRunStatus.Pending,
        CancellationToken cancellationToken = default);
}
using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;

namespace MediaBedrock.Application.JobRuns.Interfaces;

public interface IJobRunService
{
    Task<Result<JobRunId>> CreateAsync(JobId jobId, CancellationToken cancellationToken = new());
}
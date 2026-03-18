using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.JobRuns;
using MediaBedrock.Controller.Domain.Jobs;

namespace MediaBedrock.Controller.Application.JobRuns.Interfaces;

public interface IJobRunService
{
    Task<Result<JobRunId>> CreateAsync(JobId jobId, CancellationToken cancellationToken = new());
}
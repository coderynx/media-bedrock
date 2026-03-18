using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.JobRuns;

namespace MediaBedrock.Controller.Application.JobRuns.Interfaces;

public interface IJobRunStepsService
{
    Task<Result<JobRunStep>> GetAsync(JobRunStepId id, CancellationToken cancellationToken = new());
}
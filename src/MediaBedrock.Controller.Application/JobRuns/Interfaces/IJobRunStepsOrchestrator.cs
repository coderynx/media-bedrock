using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.JobRuns;

namespace MediaBedrock.Controller.Application.JobRuns.Interfaces;

public interface IJobRunStepsOrchestrator
{
    Task<Result> CompleteAsync(JobRunStepId jobRunStepId, CancellationToken cancellationToken = new());

    Task<Result> FailAsync(
        JobRunStepId jobRunStepId,
        JobStepFailureReason reason,
        string message = "",
        CancellationToken cancellationToken = new());
}
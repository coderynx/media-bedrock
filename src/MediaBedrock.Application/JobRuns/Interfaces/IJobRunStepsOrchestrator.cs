using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.JobRuns.Interfaces;

public interface IJobRunStepsOrchestrator
{
    Task<Result> StartAsync(JobRunId jobRunId, JobRunStepId jobRunStepId, CancellationToken cancellationToken = new());

    Task<Result> CompleteAsync(
        JobRunStepId jobRunStepId,
        List<JobAssetName> updatedAssetNames,
        CancellationToken cancellationToken = new());

    Task<Result> FailAsync(
        JobRunStepId jobRunStepId,
        JobStepFailureReason reason,
        string message = "",
        CancellationToken cancellationToken = new());
}
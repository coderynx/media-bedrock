using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.JobRuns.Interfaces;

public interface IJobRunsOrchestrator
{
    Task<Result> StartAsync(JobRunId jobRunId, CancellationToken cancellationToken = new());

    Task<Result> FailAsync(
        JobRunId jobRunId,
        JobFailureReason failureReason,
        string message,
        CancellationToken cancellationToken = new());

    Task<Result> CompleteAsync(JobRunId jobRunId, CancellationToken cancellationToken = new());

    Task<Result> WaitForCompletionAsync(
        JobRunId runId,
        TimeSpan delayTime,
        CancellationToken cancellationToken = new());
}
namespace MediaBedrock.Contracts.JobRuns;

public sealed record JobRunStepFailed(
    Guid JobRunId,
    Guid JobRunStepId,
    string FailureReason,
    string Message = "")
    : JobRunMessageBase(JobRunId);
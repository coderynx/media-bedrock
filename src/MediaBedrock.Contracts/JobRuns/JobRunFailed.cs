namespace MediaBedrock.Contracts.JobRuns;

public sealed record JobRunFailed(Guid JobRunId, string FailureReason, string Message = "");
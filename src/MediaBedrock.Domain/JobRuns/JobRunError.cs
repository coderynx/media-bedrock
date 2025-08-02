namespace MediaBedrock.Domain.JobRuns;

public sealed record JobRunError(JobFailureReason FailureReason, string Message = "");
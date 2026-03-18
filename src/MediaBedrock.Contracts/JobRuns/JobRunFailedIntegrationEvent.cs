namespace MediaBedrock.Contracts.JobRuns;

public sealed record JobRunFailedIntegrationEvent(Guid JobRunId, string FailureReason, string Message = "");
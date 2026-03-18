namespace MediaBedrock.Contracts.JobRuns;

public sealed record JobRunStepFailedIntegrationEvent(
    Guid JobRunId,
    Guid JobRunStepId,
    string FailureReason,
    string Message = "");
namespace MediaBedrock.Domain.JobStateMachines;

public sealed record JobExecutionError(JobFailureReason FailureReason, string Message = "");
using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.JobRuns.Messages;

public sealed record JobRunStepFailed : JobRunMessageBase
{
    public JobRunStepFailed(
        JobRunId jobRunId,
        JobRunStepId jobRunStepId,
        JobStepFailureReason failureReason = JobStepFailureReason.Unknown,
        string message = "") : base(jobRunId)
    {
        JobRunStepId = jobRunStepId;
        FailureReason = failureReason;
        Message = message;
    }

    public JobRunStepId JobRunStepId { get; }
    public JobStepFailureReason FailureReason { get; }
    public string Message { get; }
}
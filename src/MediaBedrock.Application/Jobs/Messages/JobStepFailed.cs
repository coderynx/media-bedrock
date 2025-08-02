using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.Jobs.Messages;

public sealed record JobStepFailed : JobMessageBase
{
    public JobStepFailed(
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
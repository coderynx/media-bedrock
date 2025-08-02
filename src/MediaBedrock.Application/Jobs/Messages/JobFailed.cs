using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.Jobs.Messages;

public sealed record JobFailed : JobMessageBase
{
    public JobFailed(
        JobRunId jobRunId,
        JobFailureReason failureReason,
        string message = "") : base(jobRunId)
    {
        FailureReason = failureReason;
        Message = message;
    }

    public JobFailureReason FailureReason { get; }
    public string Message { get; }
}
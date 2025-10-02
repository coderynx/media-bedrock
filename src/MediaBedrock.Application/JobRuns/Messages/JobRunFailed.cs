using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.JobRuns.Messages;

public sealed record JobRunFailed : JobRunMessageBase
{
    public JobRunFailed(
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
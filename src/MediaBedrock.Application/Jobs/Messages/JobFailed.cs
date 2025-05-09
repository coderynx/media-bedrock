using MediaBedrock.Domain.JobStateMachine;

namespace MediaBedrock.Application.Jobs.Messages;

public sealed record JobFailed : JobMessageBase
{
    public JobFailed(
        JobStateMachineId jobStateMachineId,
        JobFailureReason failureReason,
        string message = "") : base(jobStateMachineId)
    {
        FailureReason = failureReason;
        Message = message;
    }

    public JobFailureReason FailureReason { get; } = JobFailureReason.Unknown;
    public string Message { get; }
}
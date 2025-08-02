using MediaBedrock.Domain.JobStateMachines;

namespace MediaBedrock.Application.Jobs.Messages;

public sealed record JobStepFailed : JobMessageBase
{
    public JobStepFailed(
        JobStateMachineId jobStateMachineId,
        JobStepStateMachineId jobStepStateMachineId,
        JobStepFailureReason failureReason = JobStepFailureReason.Unknown,
        string message = "") : base(jobStateMachineId)
    {
        JobStepStateMachineId = jobStepStateMachineId;
        FailureReason = failureReason;
        Message = message;
    }

    public JobStepStateMachineId JobStepStateMachineId { get; }
    public JobStepFailureReason FailureReason { get; }
    public string Message { get; }
}
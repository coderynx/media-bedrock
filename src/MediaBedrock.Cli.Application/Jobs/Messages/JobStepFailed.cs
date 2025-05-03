using MediaBedrock.Cli.Domain.JobsStateMachine;

namespace MediaBedrock.Cli.Application.Jobs.Messages;

public sealed record JobStepFailed : JobMessageBase
{
    public required JobStepStateMachineId JobStepStateMachineId { get; init; }
    public required JobStepFailureReason FailureReason { get; init; }
    public string Message { get; init; } = string.Empty;

    public static JobStepFailed Create(
        JobStateMachineId jobStateMachineId,
        JobStepStateMachineId jobStepStateMachineId,
        JobStepFailureReason failureReason,
        string message = "")
    {
        return new JobStepFailed
        {
            JobStateMachineId = jobStateMachineId,
            JobStepStateMachineId = jobStepStateMachineId,
            FailureReason = failureReason,
            Message = message
        };
    }
}
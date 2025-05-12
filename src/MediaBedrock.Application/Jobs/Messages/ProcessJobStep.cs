using MediaBedrock.Domain.JobStateMachine;

namespace MediaBedrock.Application.Jobs.Messages;

public sealed record ProcessJobStep : JobMessageBase
{
    public ProcessJobStep(
        JobStateMachineId jobStateMachineId,
        JobStepStateMachineId jobJobStepStateMachineId) : base(jobStateMachineId)
    {
        JobStepStateMachineId = jobJobStepStateMachineId;
    }

    public JobStepStateMachineId JobStepStateMachineId { get; }
}
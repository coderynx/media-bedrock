using MediaBedrock.Cli.Domain.JobsStateMachine;

namespace MediaBedrock.Cli.Application.Jobs.Messages;

public sealed record ProcessJobStep : JobMessageBase
{
    public required JobStepStateMachineId StepStateMachineId { get; init; }

    public static ProcessJobStep Create(JobStateMachineId stateMachineId, JobStepStateMachineId stepStateMachineId)
    {
        return new ProcessJobStep
        {
            JobStateMachineId = stateMachineId,
            StepStateMachineId = stepStateMachineId
        };
    }
}
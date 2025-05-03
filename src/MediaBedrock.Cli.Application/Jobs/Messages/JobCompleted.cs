using MediaBedrock.Cli.Domain.JobsStateMachine;

namespace MediaBedrock.Cli.Application.Jobs.Messages;

public sealed record JobCompleted : JobMessageBase
{
    public static JobCompleted Create(JobStateMachineId jobStateMachineId)
    {
        return new JobCompleted
        {
            JobStateMachineId = jobStateMachineId
        };
    }
}
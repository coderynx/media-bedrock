using MediaBedrock.Domain.JobStateMachine;

namespace MediaBedrock.Application.Jobs.Messages;

public sealed record RunJob : JobMessageBase
{
    public RunJob(JobStateMachineId jobStateMachineId) : base(jobStateMachineId)
    {
    }
}
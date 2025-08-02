using MediaBedrock.Domain.JobStateMachines;

namespace MediaBedrock.Application.Jobs.Messages;

public sealed record JobCompleted : JobMessageBase
{
    public JobCompleted(JobStateMachineId jobStateMachineId) : base(jobStateMachineId)
    {
    }
}
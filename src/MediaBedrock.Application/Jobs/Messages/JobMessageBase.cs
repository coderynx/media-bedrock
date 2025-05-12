using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Domain.JobStateMachine;

namespace MediaBedrock.Application.Jobs.Messages;

public abstract record JobMessageBase : IMessage
{
    protected JobMessageBase(JobStateMachineId jobStateMachineId)
    {
        JobStateMachineId = jobStateMachineId;
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public JobStateMachineId JobStateMachineId { get; }
}
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.JobsStateMachine;

namespace MediaBedrock.Cli.Application.Jobs.Messages;

public abstract record JobMessageBase : IMessage
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public required JobStateMachineId JobStateMachineId { get; init; }
}
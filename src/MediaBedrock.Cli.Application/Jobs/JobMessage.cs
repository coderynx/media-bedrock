using MediaBedrock.Cli.Domain.Jobs;

namespace MediaBedrock.Cli.Application.Jobs;

public abstract record JobMessage
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public required JobId JobId { get; init; }
}
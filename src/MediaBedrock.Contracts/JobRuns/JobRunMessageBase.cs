namespace MediaBedrock.Contracts.JobRuns;

public abstract record JobRunMessageBase(Guid JobRunId) : IMessage
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
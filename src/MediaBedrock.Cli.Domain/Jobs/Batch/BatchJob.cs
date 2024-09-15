namespace MediaBedrock.Cli.Domain.Jobs.Batch;

public sealed class BatchJob
{
    public required string JobName { get; init; }
    public required BatchJobEntry[] Entries { get; init; } = [];
}
namespace MediaBedrock.Cli.Domain.Jobs.Processors;

public sealed class ProcessorConfiguration
{
    public required string Name { get; init; }
    public Dictionary<string, object?> Settings { get; init; } = new();
}
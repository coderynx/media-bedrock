namespace MediaBedrock.Cli.Domain.Processors;

public sealed class ProcessorConfiguration
{
    public required string Name { get; init; }
    public Dictionary<string, string?> Settings { get; init; } = new();
}
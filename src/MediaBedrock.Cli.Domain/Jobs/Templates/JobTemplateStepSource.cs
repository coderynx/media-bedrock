namespace MediaBedrock.Cli.Domain.Jobs.Templates;

public sealed record JobTemplateStepSource
{
    public required string Name { get; init; }
    public required string Destination { get; init; }
}
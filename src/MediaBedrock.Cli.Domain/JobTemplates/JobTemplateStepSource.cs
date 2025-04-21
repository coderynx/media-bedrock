namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed record JobTemplateStepSource
{
    public required string Name { get; init; }
    public required string Destination { get; init; }
}
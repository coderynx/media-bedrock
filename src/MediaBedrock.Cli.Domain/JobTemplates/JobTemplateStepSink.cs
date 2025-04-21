namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed record JobTemplateStepSink
{
    public required string Name { get; init; }
    public required string Source { get; init; }
}
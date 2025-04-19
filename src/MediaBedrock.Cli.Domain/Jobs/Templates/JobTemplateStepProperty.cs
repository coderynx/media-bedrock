namespace MediaBedrock.Cli.Domain.Jobs.Templates;

public sealed record JobTemplateStepProperty
{
    public required string Name { get; init; }
    public required string Value { get; init; }
}
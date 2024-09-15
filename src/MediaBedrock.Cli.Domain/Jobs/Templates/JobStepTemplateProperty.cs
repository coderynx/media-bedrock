namespace MediaBedrock.Cli.Domain.Jobs.Templates;

public sealed record JobStepTemplateProperty
{
    public required string Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public required string Value { get; init; }
}
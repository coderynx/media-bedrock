namespace MediaBedrock.Cli.Domain.Jobs.Templates;

public sealed record JobTemplateProperty
{
    public required string Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DefaultValue { get; init; } = string.Empty;
}
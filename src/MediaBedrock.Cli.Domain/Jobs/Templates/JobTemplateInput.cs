namespace MediaBedrock.Cli.Domain.Jobs.Templates;

public sealed record JobTemplateInput
{
    public required string Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
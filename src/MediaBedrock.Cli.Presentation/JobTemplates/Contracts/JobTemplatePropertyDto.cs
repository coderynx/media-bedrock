namespace MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

internal sealed record JobTemplatePropertyDto
{
    public required string Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string DefaultValue { get; init; } = string.Empty;
}
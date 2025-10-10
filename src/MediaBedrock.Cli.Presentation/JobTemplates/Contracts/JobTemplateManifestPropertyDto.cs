namespace MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

public sealed record JobTemplateManifestPropertyDto
{
    public required string Name { get; init; }
    public string DefaultValue { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
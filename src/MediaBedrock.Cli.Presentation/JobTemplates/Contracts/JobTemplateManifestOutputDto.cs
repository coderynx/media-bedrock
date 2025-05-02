namespace MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

public sealed record JobTemplateManifestOutputDto
{
    public required string Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
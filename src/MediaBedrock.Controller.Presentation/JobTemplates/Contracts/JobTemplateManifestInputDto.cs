namespace MediaBedrock.Controller.Presentation.JobTemplates.Contracts;

public sealed record JobTemplateManifestInputDto
{
    public required string Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
namespace MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

internal sealed record JobTemplateOutputDto
{
    public required string Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
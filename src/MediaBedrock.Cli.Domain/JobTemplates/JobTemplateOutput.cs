namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed record JobTemplateOutput
{
    public required string Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}
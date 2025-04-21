namespace MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

internal sealed record JobTemplateStepDto
{
    public required string Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public required string ProcessorName { get; init; }
    public Dictionary<string, string> Sinks { get; init; } = [];
    public Dictionary<string, string> Sources { get; init; } = [];
    public Dictionary<string, string> Properties { get; init; } = [];
}
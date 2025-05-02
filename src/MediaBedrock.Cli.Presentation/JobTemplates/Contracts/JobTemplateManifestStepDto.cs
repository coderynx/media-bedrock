namespace MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

public sealed record JobTemplateManifestStepDto
{
    public required string Name { get; init; }
    public required string ProcessorName { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public required IReadOnlyDictionary<string, string> Sinks { get; init; }
    public required IReadOnlyDictionary<string, string> Sources { get; init; }
    public required IReadOnlyDictionary<string, string> Properties { get; init; }
}
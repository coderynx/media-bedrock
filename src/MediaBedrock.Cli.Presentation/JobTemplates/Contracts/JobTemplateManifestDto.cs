namespace MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

public sealed record JobTemplateManifestDto
{
    public required string Name { get; init; }
    public required string Version { get; init; }
    public required string Author { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IEnumerable<JobTemplateManifestPropertyDto> Properties { get; init; } = [];
    public IEnumerable<JobTemplateManifestInputDto> Inputs { get; init; } = [];
    public IEnumerable<JobTemplateManifestOutputDto> Outputs { get; init; } = [];
    public IEnumerable<JobTemplateManifestStepDto> Steps { get; init; } = [];
}
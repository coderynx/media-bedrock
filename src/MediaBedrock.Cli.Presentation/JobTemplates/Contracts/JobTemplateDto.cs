namespace MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

internal sealed record JobTemplateDto
{
    public required string Name { get; init; }
    public string Version { get; init; } = "0.0.1";
    public string Author { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public JobTemplatePropertyDto[] Properties { get; init; } = [];
    public JobTemplateInputDto[] Inputs { get; init; } = [];
    public JobTemplateOutputDto[] Outputs { get; init; } = [];
    public JobTemplateStepDto[] Steps { get; init; } = [];
}
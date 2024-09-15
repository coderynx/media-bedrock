namespace MediaBedrock.Cli.Domain.Jobs.Templates;

public sealed record JobTemplate
{
    public required JobTemplateName Name { get; init; }
    public required string Version { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public JobTemplateProperty[] Parameters { get; init; } = [];
    public JobTemplateInput[] Inputs { get; init; } = [];
    public JobTemplateOutput[] Outputs { get; init; } = [];
    public JobTemplateStep[] Steps { get; init; } = [];
}
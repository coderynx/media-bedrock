namespace MediaBedrock.Cli.Domain.Jobs.Templates;

public sealed class JobTemplate
{
    public required JobTemplateName Name { get; init; }
    public JobTemplateAuthor Author { get; init; } = JobTemplateAuthor.Empty;
    public JobTemplateVersion Version { get; init; } = JobTemplateVersion.Default;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public JobTemplateProperty[] Properties { get; init; } = [];
    public JobTemplateInput[] Inputs { get; init; } = [];
    public JobTemplateOutput[] Outputs { get; init; } = [];
    public JobTemplateStep[] Steps { get; init; } = [];
}
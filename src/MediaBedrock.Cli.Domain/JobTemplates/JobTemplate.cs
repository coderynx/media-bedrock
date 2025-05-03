namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed class JobTemplate
{
    private readonly List<JobTemplateStep> _steps = [];
    private List<JobTemplateInput> _inputs = [];
    private List<JobTemplateOutput> _outputs = [];
    private List<JobTemplateProperty> _properties = [];

    private JobTemplate()
    {
    }

    public required JobTemplateId Id { get; init; }
    public required JobTemplateName Name { get; init; }
    public JobTemplateVersion Version { get; init; } = new();
    public JobTemplateAuthor Author { get; init; } = new();
    public JobTemplateDisplayName DisplayName { get; init; } = new();
    public JobTemplateDescription Description { get; init; } = new();
    public IReadOnlyList<JobTemplateInput> Inputs => _inputs;
    public IReadOnlyList<JobTemplateOutput> Outputs => _outputs;
    public IReadOnlyList<JobTemplateProperty> Properties => _properties;
    public IReadOnlyList<JobTemplateStep> Steps => _steps;

    public static JobTemplate Create(
        JobTemplateName name,
        JobTemplateVersion version,
        JobTemplateAuthor author,
        List<JobTemplateInput> inputs,
        List<JobTemplateOutput> outputs,
        List<JobTemplateProperty> properties,
        JobTemplateDisplayName? displayName = null,
        JobTemplateDescription? description = null)
    {
        return new JobTemplate
        {
            Id = new JobTemplateId(),
            Name = name,
            Version = version,
            Author = author,
            _inputs = inputs,
            _outputs = outputs,
            _properties = properties,
            DisplayName = displayName ?? new JobTemplateDisplayName(),
            Description = description ?? new JobTemplateDescription()
        };
    }

    public void AddStepRange(List<JobTemplateStep> steps)
    {
        _steps.AddRange(steps);
    }
}
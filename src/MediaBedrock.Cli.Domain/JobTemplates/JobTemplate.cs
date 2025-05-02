using MediaBedrock.Cli.Domain.Jobs;

namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed class JobTemplate
{
    private readonly List<Job> _jobs = [];
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
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<JobTemplateInput> Inputs => _inputs.AsReadOnly();
    public IReadOnlyList<JobTemplateOutput> Outputs => _outputs.AsReadOnly();
    public IReadOnlyList<JobTemplateProperty> Properties => _properties.AsReadOnly();
    public IReadOnlyList<JobTemplateStep> Steps => _steps.AsReadOnly();
    public IReadOnlyList<Job> Jobs => _jobs.AsReadOnly();

    public static JobTemplate Create(
        JobTemplateName name,
        JobTemplateVersion version,
        JobTemplateAuthor author,
        List<JobTemplateInput> inputs,
        List<JobTemplateOutput> outputs,
        List<JobTemplateProperty> properties,
        string displayName = "",
        string description = "")
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
            DisplayName = displayName,
            Description = description
        };
    }

    public void AddStepRange(List<JobTemplateStep> steps)
    {
        _steps.AddRange(steps);
    }
}
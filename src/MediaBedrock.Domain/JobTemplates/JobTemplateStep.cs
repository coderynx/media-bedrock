using MediaBedrock.Domain.Processors;

namespace MediaBedrock.Domain.JobTemplates;

public sealed class JobTemplateStep
{
    private List<JobTemplateStepInput> _inputs = [];
    private List<JobTemplateStepOutput> _outputs = [];
    private List<JobTemplateStepProperty> _properties = [];

    private JobTemplateStep()
    {
    }

    public required JobTemplateStepId Id { get; init; }
    public required JobTemplate Template { get; init; }
    public required JobTemplateStepOrder Order { get; init; }
    public required JobTemplateStepName Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public required ProcessorName ProcessorName { get; init; }

    public IReadOnlyList<JobTemplateStepInput> Inputs => _inputs.AsReadOnly();
    public IReadOnlyList<JobTemplateStepOutput> Outputs => _outputs.AsReadOnly();
    public IReadOnlyList<JobTemplateStepProperty> Properties => _properties.AsReadOnly();

    public static JobTemplateStep Create(
        JobTemplate template,
        JobTemplateStepOrder order,
        JobTemplateStepName name,
        ProcessorName processorName,
        List<JobTemplateStepInput> inputs,
        List<JobTemplateStepOutput> outputs,
        List<JobTemplateStepProperty> properties,
        string displayName = "",
        string description = "")
    {
        return new JobTemplateStep
        {
            Id = new JobTemplateStepId(),
            Template = template,
            Order = order,
            Name = name,
            ProcessorName = processorName,
            _inputs = inputs,
            _outputs = outputs,
            _properties = properties,
            DisplayName = displayName,
            Description = description
        };
    }
}
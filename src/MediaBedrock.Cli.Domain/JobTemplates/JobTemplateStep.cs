using MediaBedrock.Cli.Domain.Processors;

namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed class JobTemplateStep
{
    private List<JobTemplateStepProperty> _properties = [];
    private List<JobTemplateStepSink> _sinks = [];
    private List<JobTemplateStepSource> _sources = [];

    private JobTemplateStep()
    {
    }

    public required JobTemplateStepId Id { get; init; }
    public required JobTemplate Template { get; init; }
    public required JobTemplateStepName Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public required ProcessorName ProcessorName { get; init; }

    public IReadOnlyList<JobTemplateStepSink> Sinks => _sinks.AsReadOnly();
    public IReadOnlyList<JobTemplateStepSource> Sources => _sources.AsReadOnly();
    public IReadOnlyList<JobTemplateStepProperty> Properties => _properties.AsReadOnly();

    public static JobTemplateStep Create(
        JobTemplate template,
        JobTemplateStepName name,
        ProcessorName processorName,
        List<JobTemplateStepSink> sinks,
        List<JobTemplateStepSource> sources,
        List<JobTemplateStepProperty> properties,
        string displayName = "",
        string description = "")
    {
        return new JobTemplateStep
        {
            Id = new JobTemplateStepId(),
            Template = template,
            Name = name,
            ProcessorName = processorName,
            _sinks = sinks,
            _sources = sources,
            _properties = properties,
            DisplayName = displayName,
            Description = description
        };
    }
}
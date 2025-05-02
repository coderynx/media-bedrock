using MediaBedrock.Cli.Domain.Processors;

namespace MediaBedrock.Cli.Domain.JobTemplates.Manifests;

public sealed record JobTemplateManifestStep
{
    public JobTemplateManifestStep(
        JobTemplateStepName name,
        ProcessorName processorName,
        IReadOnlyDictionary<string, string> sinksMappings,
        IReadOnlyDictionary<string, string> sourcesMappings,
        IReadOnlyDictionary<string, string> properties,
        string displayName = "",
        string description = "")
    {
        Name = name;
        ProcessorName = processorName;
        DisplayName = displayName;
        Description = description;
        SinksMappings = sinksMappings;
        SourcesMappings = sourcesMappings;
        Properties = properties;
    }

    public JobTemplateStepName Name { get; init; }
    public ProcessorName ProcessorName { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, string> SinksMappings { get; }
    public IReadOnlyDictionary<string, string> SourcesMappings { get; }
    public IReadOnlyDictionary<string, string> Properties { get; }

    public JobTemplateStep ToTemplateStep(JobTemplate template)
    {
        return JobTemplateStep.Create(
            template: template,
            name: Name,
            processorName: ProcessorName,
            sinks: SinksMappings.Select(i => new JobTemplateStepSink(i.Key, i.Value)).ToList(),
            sources: SourcesMappings.Select(o => new JobTemplateStepSource(o.Key, o.Value)).ToList(),
            properties: Properties.Select(p => new JobTemplateStepProperty(p.Key, p.Value)).ToList(),
            displayName: DisplayName,
            description: Description);
    }
}
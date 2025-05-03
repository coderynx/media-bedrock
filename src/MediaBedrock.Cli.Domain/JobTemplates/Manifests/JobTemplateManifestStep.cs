using MediaBedrock.Cli.Domain.Processors;

namespace MediaBedrock.Cli.Domain.JobTemplates.Manifests;

public sealed record JobTemplateManifestStep
{
    public JobTemplateManifestStep(
        JobTemplateStepName name,
        ProcessorName processorName,
        IReadOnlyDictionary<string, string> inputsMappings,
        IReadOnlyDictionary<string, string> outputsMappings,
        IReadOnlyDictionary<string, string> properties,
        string displayName = "",
        string description = "")
    {
        Name = name;
        ProcessorName = processorName;
        DisplayName = displayName;
        Description = description;
        InputsMappings = inputsMappings;
        OutputsMappings = outputsMappings;
        Properties = properties;
    }

    public JobTemplateStepName Name { get; init; }
    public ProcessorName ProcessorName { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, string> InputsMappings { get; }
    public IReadOnlyDictionary<string, string> OutputsMappings { get; }
    public IReadOnlyDictionary<string, string> Properties { get; }

    public JobTemplateStep ToTemplateStep(JobTemplate template)
    {
        return JobTemplateStep.Create(
            template: template,
            name: Name,
            processorName: ProcessorName,
            inputs: InputsMappings.Select(i => new JobTemplateStepInput(i.Key, i.Value)).ToList(),
            outputs: OutputsMappings.Select(o => new JobTemplateStepOutput(o.Key, o.Value)).ToList(),
            properties: Properties.Select(p => new JobTemplateStepProperty(p.Key, p.Value)).ToList(),
            displayName: DisplayName,
            description: Description);
    }
}
namespace MediaBedrock.Cli.Domain.JobTemplates.Manifests;

public sealed record JobTemplateManifest
{
    public JobTemplateManifest(
        JobTemplateName name,
        JobTemplateVersion version,
        JobTemplateAuthor author,
        List<JobTemplateManifestProperty> properties,
        List<JobTemplateManifestInput> inputs,
        List<JobTemplateManifestOutput> outputs,
        List<JobTemplateManifestStep> steps,
        string displayName = "",
        string description = "")
    {
        Name = name;
        Version = version;
        Author = author;
        Properties = properties.AsReadOnly();
        Inputs = inputs.AsReadOnly();
        Outputs = outputs.AsReadOnly();
        Steps = steps.AsReadOnly();
        DisplayName = displayName;
        Description = description;
    }

    public JobTemplateName Name { get; }
    public JobTemplateVersion Version { get; }
    public JobTemplateAuthor Author { get; }
    public string DisplayName { get; }
    public string Description { get; }
    public IReadOnlyList<JobTemplateManifestProperty> Properties { get; }
    public IReadOnlyList<JobTemplateManifestInput> Inputs { get; }
    public IReadOnlyList<JobTemplateManifestOutput> Outputs { get; }
    public IReadOnlyList<JobTemplateManifestStep> Steps { get; }

    public JobTemplate ToTemplate()
    {
        var template = JobTemplate.Create(
            name: Name,
            version: Version,
            author: Author,
            inputs: Inputs.Select(i => i.ToTemplateInput()).ToList(),
            outputs: Outputs.Select(o => o.ToTemplateOutput()).ToList(),
            properties: Properties.Select(o => o.ToTemplateProperty()).ToList(),
            displayName: DisplayName,
            description: Description);

        var steps = Steps.Select(s => s.ToTemplateStep(template)).ToList();
        template.AddStepRange(steps);

        return template;
    }
}
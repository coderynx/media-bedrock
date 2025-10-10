namespace MediaBedrock.Domain.JobTemplates.Manifests;

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
        JobTemplateDisplayName? displayName = null,
        JobTemplateDescription? description = null)
    {
        Name = name;
        Version = version;
        Author = author;
        Properties = properties.AsReadOnly();
        Inputs = inputs.AsReadOnly();
        Outputs = outputs.AsReadOnly();
        Steps = steps.AsReadOnly();
        DisplayName = displayName ?? new JobTemplateDisplayName();
        Description = description ?? new JobTemplateDescription();
    }

    public JobTemplateName Name { get; }
    public JobTemplateVersion Version { get; }
    public JobTemplateAuthor Author { get; }
    public JobTemplateDisplayName DisplayName { get; }
    public JobTemplateDescription Description { get; }
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

        var steps = Steps
            .Select((step, index) => step.ToTemplateStep(template, new JobTemplateStepOrder((uint)(index + 1))))
            .ToList();

        template.AddStepRange(steps);

        return template;
    }
}
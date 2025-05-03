namespace MediaBedrock.Cli.Domain.JobTemplates.Manifests;

public sealed record JobTemplateManifestOutput
{
    public JobTemplateManifestOutput(string name, string displayName = "", string description = "")
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
    }

    public string Name { get; }
    public string DisplayName { get; }
    public string Description { get; }

    public JobTemplateOutput ToTemplateOutput()
    {
        return new JobTemplateOutput(Name, DisplayName, Description);
    }
}
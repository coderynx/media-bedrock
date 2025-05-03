namespace MediaBedrock.Cli.Domain.JobTemplates.Manifests;

public sealed record JobTemplateManifestProperty
{
    public JobTemplateManifestProperty(
        string name,
        string defaultValue,
        string displayName = "",
        string description = "")
    {
        Name = name;
        DefaultValue = defaultValue;
        DisplayName = displayName;
        Description = description;
    }

    public string Name { get; }
    public string DefaultValue { get; }
    public string DisplayName { get; }
    public string Description { get; }

    public JobTemplateProperty ToTemplateProperty()
    {
        return new JobTemplateProperty(Name, DefaultValue, DisplayName, Description);
    }
}
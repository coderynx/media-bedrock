namespace MediaBedrock.Controller.Domain.JobTemplates.Manifests;

public sealed record JobTemplateManifestInput
{
    public JobTemplateManifestInput(string name, string displayName = "", string description = "")
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
    }

    public string Name { get; }
    public string DisplayName { get; }
    public string Description { get; }

    public JobTemplateInput ToTemplateInput()
    {
        return new JobTemplateInput(Name, DisplayName, Description);
    }
}
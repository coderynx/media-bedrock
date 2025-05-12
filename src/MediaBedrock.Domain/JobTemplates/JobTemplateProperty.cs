namespace MediaBedrock.Domain.JobTemplates;

public sealed record JobTemplateProperty
{
    internal JobTemplateProperty(
        string name,
        string defaultValue = "",
        string displayName = "",
        string description = "")
    {
        Name = name;
        DefaultValue = defaultValue;
        DisplayName = displayName;
        Description = description;
    }

    public string Name { get; init; }
    public string DefaultValue { get; init; }
    public string DisplayName { get; init; }
    public string Description { get; init; }
}
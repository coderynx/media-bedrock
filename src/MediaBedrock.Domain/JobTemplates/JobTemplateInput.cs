namespace MediaBedrock.Domain.JobTemplates;

public sealed record JobTemplateInput
{
    internal JobTemplateInput(string name, string displayName = "", string description = "")
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
    }

    public string Name { get; init; }
    public string DisplayName { get; init; }
    public string Description { get; init; }
}
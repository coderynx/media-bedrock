namespace MediaBedrock.Domain.JobTemplates;

public sealed record JobTemplateStepInput
{
    internal JobTemplateStepInput(string name, string source)
    {
        Name = name;
        Source = source;
    }

    public string Name { get; init; }
    public string Source { get; init; }
}
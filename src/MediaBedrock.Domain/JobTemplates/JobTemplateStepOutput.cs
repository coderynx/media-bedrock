namespace MediaBedrock.Domain.JobTemplates;

public sealed record JobTemplateStepOutput
{
    internal JobTemplateStepOutput(string name, string destination)
    {
        Name = name;
        Destination = destination;
    }

    public string Name { get; init; }
    public string Destination { get; init; }
}
namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed record JobTemplateStepSource
{
    internal JobTemplateStepSource(string name, string destination)
    {
        Name = name;
        Destination = destination;
    }

    public string Name { get; init; }
    public string Destination { get; init; }
}
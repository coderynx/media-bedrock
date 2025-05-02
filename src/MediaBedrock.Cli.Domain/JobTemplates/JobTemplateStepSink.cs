namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed record JobTemplateStepSink
{
    internal JobTemplateStepSink(string name, string source)
    {
        Name = name;
        Source = source;
    }

    public string Name { get; init; }
    public string Source { get; init; }
}
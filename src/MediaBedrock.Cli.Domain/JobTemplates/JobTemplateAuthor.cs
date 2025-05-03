namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed record JobTemplateAuthor
{
    public JobTemplateAuthor(string value)
    {
        Value = value;
    }

    public JobTemplateAuthor()
    {
        Value = string.Empty;
    }

    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }
}
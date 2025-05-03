namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed record JobTemplateDescription(string Value = "")
{
    public override string ToString()
    {
        return Value;
    }
}
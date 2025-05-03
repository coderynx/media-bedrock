namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed record JobTemplateDisplayName(string Value = "")
{
    public override string ToString()
    {
        return Value;
    }
}
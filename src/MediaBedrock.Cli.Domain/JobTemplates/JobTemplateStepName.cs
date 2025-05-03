namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed record JobTemplateStepName(string Value)
{
    public override string ToString()
    {
        return Value;
    }
}
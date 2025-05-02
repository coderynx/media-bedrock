namespace MediaBedrock.Cli.Domain.JobTemplates;

/// <summary>
///     Represents the name of a job template.
/// </summary>
public sealed record JobTemplateName(string Value)
{
    public override string ToString()
    {
        return Value;
    }
}
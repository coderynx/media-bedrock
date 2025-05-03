namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed record JobTemplateStepId(Guid Value)
{
    public JobTemplateStepId() : this(Guid.CreateVersion7())
    {
    }
}
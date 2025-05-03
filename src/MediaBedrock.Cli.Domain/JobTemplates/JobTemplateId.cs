namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed record JobTemplateId
{
    public JobTemplateId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException(JobTemplateErrors.InvalidIdCode, "Job template ID cannot be empty.");
        }

        Value = value;
    }

    public JobTemplateId()
    {
        Value = Guid.CreateVersion7();
    }

    public Guid Value { get; }
}
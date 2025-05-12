namespace MediaBedrock.Domain.JobTemplates;

public sealed record JobTemplateId
{
    public JobTemplateId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw JobTemplateErrors.InvalidId();
        }

        Value = value;
    }

    public JobTemplateId()
    {
        Value = Guid.CreateVersion7();
    }

    public Guid Value { get; }
}
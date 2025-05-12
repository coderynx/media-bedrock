namespace MediaBedrock.Domain.JobTemplates;

public sealed record JobTemplateStepId
{
    public JobTemplateStepId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw JobTemplateErrors.InvalidStepId();
        }

        Value = value;
    }

    public JobTemplateStepId()
    {
        Value = Guid.CreateVersion7();
    }

    public Guid Value { get; }

    public override string ToString()
    {
        return Value.ToString();
    }
}
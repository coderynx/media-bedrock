using MediaBedrock.Domain.JobTemplates;

namespace MediaBedrock.Domain.Jobs;

public sealed record JobOutputId
{
    public JobOutputId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw JobTemplateErrors.InvalidInputId();
        }

        Value = value;
    }

    public JobOutputId()
    {
        Value = Guid.CreateVersion7();
    }

    public Guid Value { get; init; }
}
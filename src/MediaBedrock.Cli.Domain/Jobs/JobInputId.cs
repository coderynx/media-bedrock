using MediaBedrock.Cli.Domain.JobTemplates;

namespace MediaBedrock.Cli.Domain.Jobs;

public sealed record JobInputId
{
    public JobInputId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw JobTemplateErrors.InvalidInputId();
        }

        Value = value;
    }

    public JobInputId()
    {
        Value = Guid.CreateVersion7();
    }

    public Guid Value { get; }
}
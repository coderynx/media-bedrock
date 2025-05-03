using MediaBedrock.Cli.Domain.JobTemplates;

namespace MediaBedrock.Cli.Domain.Jobs;

public sealed record JobOutputId
{
    public JobOutputId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException(JobTemplateErrors.InvalidOutputIdCode, "Output ID cannot be empty.");
        }

        Value = value;
    }

    public JobOutputId()
    {
        Value = Guid.CreateVersion7();
    }

    public Guid Value { get; init; }
}
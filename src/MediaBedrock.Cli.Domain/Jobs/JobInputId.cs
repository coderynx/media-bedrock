using MediaBedrock.Cli.Domain.JobTemplates;

namespace MediaBedrock.Cli.Domain.Jobs;

public sealed record JobInputId
{
    public JobInputId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException(JobTemplateErrors.InvalidInputIdCode, "Input ID cannot be empty.");
        }

        Value = value;
    }

    public JobInputId()
    {
        Value = Guid.CreateVersion7();
    }

    public Guid Value { get; init; }
}
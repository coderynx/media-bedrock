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

public sealed record JobOutput
{
    private JobOutput()
    {
    }

    public required JobOutputId Id { get; init; }
    public required string Name { get; init; }
    public required string FilePath { get; init; }

    public static JobOutput Create(string name, string filePath)
    {
        return new JobOutput
        {
            Id = new JobOutputId(),
            Name = name,
            FilePath = filePath
        };
    }
}
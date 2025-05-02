namespace MediaBedrock.Cli.Domain.Jobs;

public sealed record JobInput
{
    private JobInput()
    {
    }

    public required JobInputId Id { get; init; }
    public required string Name { get; init; }
    public required string Uri { get; init; }

    public static JobInput Create(string name, string filePath)
    {
        return new JobInput
        {
            Id = new JobInputId(),
            Name = name,
            Uri = filePath
        };
    }
}
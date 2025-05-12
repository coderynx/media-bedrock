namespace MediaBedrock.Domain.Jobs;

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
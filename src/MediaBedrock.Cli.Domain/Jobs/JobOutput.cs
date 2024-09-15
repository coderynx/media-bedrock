namespace MediaBedrock.Cli.Domain.Jobs;

public sealed record JobOutput
{
    public required string Name { get; init; }
    public required string FilePath { get; init; }

    public static JobOutput Create(string name, string filePath)
    {
        return new JobOutput
        {
            Name = name,
            FilePath = filePath
        };
    }
}
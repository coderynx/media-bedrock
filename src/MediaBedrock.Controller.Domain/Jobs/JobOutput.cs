namespace MediaBedrock.Controller.Domain.Jobs;

public sealed record JobOutput
{
    private JobOutput()
    {
    }

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
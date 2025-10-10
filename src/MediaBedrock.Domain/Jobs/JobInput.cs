namespace MediaBedrock.Domain.Jobs;

public sealed record JobInput
{
    private JobInput()
    {
    }

    public required string Name { get; init; }
    public required string Uri { get; init; }

    public static JobInput Create(string name, string filePath)
    {
        return new JobInput
        {
            Name = name,
            Uri = filePath
        };
    }
}
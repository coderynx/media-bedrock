namespace MediaBedrock.Cli.Domain.Jobs.Steps;

public sealed record JobStepProperty
{
    public required string Name { get; init; }
    public required string? Value { get; init; }

    public static JobStepProperty Create(string name, string? value)
    {
        return new JobStepProperty
        {
            Name = name,
            Value = value
        };
    }
}
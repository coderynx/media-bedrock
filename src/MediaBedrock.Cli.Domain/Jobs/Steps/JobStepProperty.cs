namespace MediaBedrock.Cli.Domain.Jobs.Steps;

public sealed record JobStepProperty
{
    public required string Name { get; init; }
    public required object? Value { get; init; }

    public static JobStepProperty Create(string name, object? value)
    {
        return new JobStepProperty
        {
            Name = name,
            Value = value
        };
    }
}
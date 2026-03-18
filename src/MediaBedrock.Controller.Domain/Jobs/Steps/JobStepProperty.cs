namespace MediaBedrock.Controller.Domain.Jobs.Steps;

public sealed record JobStepProperty
{
    private JobStepProperty()
    {
    }

    public required string Name { get; init; }
    public string? Value { get; init; }

    public static JobStepProperty Create(string name, string? value)
    {
        return new JobStepProperty
        {
            Name = name,
            Value = value
        };
    }
}
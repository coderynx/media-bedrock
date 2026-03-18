using Coderynx.Functional.Results;

namespace MediaBedrock.Worker.Domain.Processing.ValueObjects;

public sealed class ProcessorInstanceProperty
{
    private ProcessorInstanceProperty()
    {
    }

    public required string Name { get; init; }
    public string? Value { get; init; }

    public static Result<ProcessorInstanceProperty> Create(string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ProcessorInstanceErrors.InvalidPropertyName(name);
        }

        var property = new ProcessorInstanceProperty
        {
            Name = name,
            Value = value
        };

        return Result.Created(property);
    }
}
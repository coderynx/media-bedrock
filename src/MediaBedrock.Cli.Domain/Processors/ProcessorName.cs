using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Processors;

public sealed record ProcessorName
{
    private ProcessorName()
    {
    }

    public required string Namespace { get; init; }
    public required string Name { get; init; }

    public static Result<ProcessorName> Create(string @namespace, string name)
    {
        if (string.IsNullOrWhiteSpace(@namespace))
        {
            return ProcessorErrors.InvalidNamespace(@namespace);
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return ProcessorErrors.InvalidName(name);
        }

        var processorName = new ProcessorName
        {
            Namespace = @namespace,
            Name = name
        };

        return Result.Created(processorName);
    }

    public static Result<ProcessorName> Create(string fullName)
    {
        var parts = fullName.Split('/');
        if (parts.Length is not 2)
        {
            return ProcessorErrors.InvalidName(fullName);
        }

        var processorName = new ProcessorName
        {
            Namespace = parts[0].Trim(),
            Name = parts[1].Trim()
        };

        return Result.Created(processorName);
    }

    public override string ToString()
    {
        return $"{Namespace}/{Name}";
    }
}
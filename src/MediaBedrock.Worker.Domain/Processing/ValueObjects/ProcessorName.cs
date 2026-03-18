using Coderynx.Functional.Results;

namespace MediaBedrock.Worker.Domain.Processing.ValueObjects;

public sealed record ProcessorName
{
    public ProcessorName(string fullName)
    {
        var parts = fullName.Split('/');
        if (parts.Length is not 2)
        {
            throw ProcessorInstanceErrors.InvalidProcessorName(fullName);
        }

        Namespace = parts[0];
        Name = parts[1];
    }

    public ProcessorName(string @namespace, string name)
    {
        if (string.IsNullOrWhiteSpace(@namespace))
        {
            throw ProcessorInstanceErrors.InvalidProcessorNamespace(@namespace);
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw ProcessorInstanceErrors.InvalidProcessorName(name);
        }

        Namespace = @namespace;
        Name = name;
    }

    public string Namespace { get; }
    public string Name { get; }

    public static Result<ProcessorName> Create(string @namespace, string name)
    {
        return Result.TryCatch(() => new ProcessorName(@namespace, name));
    }

    public static Result<ProcessorName> Create(string fullName)
    {
        return Result.TryCatch(() => new ProcessorName(fullName));
    }

    public override string ToString()
    {
        return $"{Namespace}/{Name}";
    }
}
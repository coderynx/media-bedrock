using Coderynx.Functional.Result;

namespace MediaBedrock.Cli.Domain.Jobs.Processors;

public sealed record ProcessorName(string Namespace, string Name)
{
    public static Result<ProcessorName> Create(string fullName)
    {
        var parts = fullName.Split('/');
        if (parts.Length is not 2)
        {
            return ProcessorErrors.InvalidName(fullName);
        }

        var processorName = new ProcessorName(parts[0], parts[1]);
        return Result.Created(processorName);
    }

    public override string ToString()
    {
        return $"{Namespace}/{Name}";
    }
}
using Coderynx.Functional.Results;

namespace MediaBedrock.Worker.Domain.Processing.ValueObjects;

public sealed record ProcessorInstanceOutput
{
    private ProcessorInstanceOutput()
    {
    }

    public required string Name { get; init; }
    public required Uri Uri { get; init; }

    public static Result<ProcessorInstanceOutput> Create(string name, Uri uri)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ProcessorInstanceErrors.InvalidOutputName(name);
        }

        var output = new ProcessorInstanceOutput
        {
            Name = name,
            Uri = uri
        };
        
        return Result.Created(output);
    }
}
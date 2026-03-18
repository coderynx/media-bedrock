using Coderynx.Functional.Results;

namespace MediaBedrock.Worker.Domain.Processing.ValueObjects;

public sealed record ProcessorInstanceInput
{
    private ProcessorInstanceInput()
    {
    }
    
    public required string Name { get; init; }
    public required Uri Uri { get; init; }
    public AssetInformation AssetInformation { get; init; } = new();

    public static Result<ProcessorInstanceInput> Create(string name, Uri uri, AssetInformation assetInformation)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ProcessorInstanceErrors.InvalidInputName(name);
        }
        
        var instanceInput = new ProcessorInstanceInput
        {
            Name = name,
            Uri = uri,
            AssetInformation = assetInformation
        };
        
        return Result.Created(instanceInput);
    }
}
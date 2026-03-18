namespace MediaBedrock.Worker.Domain.Processing.ValueObjects;

public sealed record AssetInformation
{
    public AssetInformation(string format)
    {
        Format = format;
    }

    public AssetInformation()
    {
    }
    
    public string Format { get; init; } = string.Empty;
}
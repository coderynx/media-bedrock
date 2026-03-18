namespace MediaBedrock.Worker.Sdk.Processors;

public sealed record ProcessorOutput
{
    private readonly Uri _uri;

    public ProcessorOutput(string name, string assetName, Uri uri)
    {
        Name = name;
        AssetName = assetName;
        _uri = uri;
    }

    public string Name { get; init; }
    public string AssetName { get; init; }

    public string GetAsFilePath()
    {
        return _uri.LocalPath;
    }

    public Stream GetAsStream()
    {
        return File.Open(GetAsFilePath(), FileMode.OpenOrCreate, FileAccess.ReadWrite);
    }
}
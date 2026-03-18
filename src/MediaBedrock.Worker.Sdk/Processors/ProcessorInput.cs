namespace MediaBedrock.Worker.Sdk.Processors;

public sealed record ProcessorInput
{
    private readonly Uri _uri;

    public ProcessorInput(string name, Uri uri, MediaInformation mediaInformation)
    {
        Name = name;
        _uri = uri;
        MediaInformation = mediaInformation;
    }

    public string Name { get; init; }
    public MediaInformation MediaInformation { get; init; }

    public string GetAsFilePath()
    {
        return _uri.LocalPath;
    }

    public Stream GetAsStream()
    {
        return File.OpenRead(GetAsFilePath());
    }
}
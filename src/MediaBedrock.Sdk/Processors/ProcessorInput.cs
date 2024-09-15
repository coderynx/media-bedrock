namespace MediaBedrock.Sdk.Processors;

public sealed record ProcessorInput
{
    private readonly Uri _uri;

    public ProcessorInput(string name, string uri, MediaInformation mediaInformation)
    {
        Name = name;
        _uri = new Uri(uri);
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
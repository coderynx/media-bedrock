namespace MediaBedrock.Dolby.Jobs.Models.Outputs;

public sealed record ManifestOutput : IJobOutput
{
    public required string FilePath { get; init; }

    public static ManifestOutputBuilder CreateBuilder()
    {
        return new ManifestOutputBuilder();
    }
}

public sealed class ManifestOutputBuilder
{
    private string _filePath = null!;

    internal ManifestOutputBuilder()
    {
    }

    public ManifestOutputBuilder WithFilePath(string filePath)
    {
        _filePath = filePath;
        return this;
    }

    public ManifestOutput Build()
    {
        return new ManifestOutput
        {
            FilePath = _filePath
        };
    }
}
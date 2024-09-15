namespace MediaBedrock.Dolby.Jobs.Models.Outputs;

public sealed record WavOutput : IJobOutput
{
    public required string FilePath { get; init; }

    public static WavOutputBuilder CreateBuilder()
    {
        return new WavOutputBuilder();
    }
}

public sealed record WavOutputBuilder
{
    private string _filePath = null!;

    internal WavOutputBuilder()
    {
    }

    public WavOutputBuilder WithFilePath(string filePath)
    {
        _filePath = filePath;
        return this;
    }

    public WavOutput Build()
    {
        return new WavOutput
        {
            FilePath = _filePath
        };
    }
}
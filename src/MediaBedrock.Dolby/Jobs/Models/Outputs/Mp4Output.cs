namespace MediaBedrock.Dolby.Jobs.Models.Outputs;

public sealed record Mp4Output : IJobOutput
{
    public required string FilePath { get; init; }
    public bool OutputAsDash { get; init; }

    public static Mp4OutputBuilder CreateBuilder()
    {
        return new Mp4OutputBuilder();
    }
}

public interface IMp4OutputBuilderInitialStage
{
    IMp4OutputBuilderFinalStage WithFilePath(string filePath);
}

public interface IMp4OutputBuilderFinalStage
{
    IMp4OutputBuilderFinalStage WithOutputAsDash(bool outputAsDash);
    Mp4Output Build();
}

public sealed class Mp4OutputBuilder :
    IMp4OutputBuilderInitialStage,
    IMp4OutputBuilderFinalStage
{
    private string _filePath = null!;
    private bool _outputAsDash;

    internal Mp4OutputBuilder()
    {
    }

    public IMp4OutputBuilderFinalStage WithOutputAsDash(bool outputAsDash)
    {
        _outputAsDash = outputAsDash;
        return this;
    }

    public Mp4Output Build()
    {
        return new Mp4Output
        {
            FilePath = _filePath,
            OutputAsDash = _outputAsDash
        };
    }

    public IMp4OutputBuilderFinalStage WithFilePath(string filePath)
    {
        _filePath = filePath;
        return this;
    }
}
namespace MediaBedrock.Dolby.Jobs.Models.Outputs;

public sealed record MlpOutput : IJobOutput
{
    public required string FilePath { get; init; }

    public static IMlpOutputBuilderInitialStage CreateBuilder()
    {
        return new MlpOutputBuilder();
    }
}

public interface IMlpOutputBuilderInitialStage
{
    MlpOutputBuilder WithFilePath(string filePath);
}

public sealed class MlpOutputBuilder : IMlpOutputBuilderInitialStage
{
    private string _filePath = null!;

    internal MlpOutputBuilder()
    {
    }

    public MlpOutputBuilder WithFilePath(string filePath)
    {
        _filePath = filePath;
        return this;
    }

    public MlpOutput Build()
    {
        return new MlpOutput
        {
            FilePath = _filePath
        };
    }
}
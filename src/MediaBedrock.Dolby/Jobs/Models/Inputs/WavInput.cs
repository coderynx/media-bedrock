using MediaBedrock.Dolby.Jobs.Models.Filters;

namespace MediaBedrock.Dolby.Jobs.Models.Inputs;

public sealed record WavInput : IJobInput
{
    public required string FilePath { get; init; }
    public required TimeCodeFrameRate TimeCodeFrameRate { get; init; } = TimeCodeFrameRate.NotIndicated;
    public required string Offset { get; init; }
    public required string FirstFrameOfAction { get; init; }

    public static IWavInputBuilderInitialStage CreateBuilder()
    {
        return new WavInputBuilder();
    }
}

public interface IWavInputBuilderInitialStage
{
    IWavInputBuilderFinalStage WithFilePath(string filePath);
}

public interface IWavInputBuilderFinalStage
{
    IWavInputBuilderFinalStage WithTimeCodeFrameRate(TimeCodeFrameRate timeCodeFrameRate);
    IWavInputBuilderFinalStage WithOffset(string offset);
    IWavInputBuilderFinalStage WithFirstFrameOfAction(string firstFrameOfAction);
    WavInput Build();
}

public sealed class WavInputBuilder : IWavInputBuilderInitialStage,
    IWavInputBuilderFinalStage
{
    private string _filePath = null!;
    private string _firstFrameOfAction = "auto";
    private string _offset = "auto";
    private TimeCodeFrameRate _timeCodeFrameRate = TimeCodeFrameRate.NotIndicated;

    internal WavInputBuilder()
    {
    }

    public IWavInputBuilderFinalStage WithTimeCodeFrameRate(TimeCodeFrameRate timeCodeFrameRate)
    {
        _timeCodeFrameRate = timeCodeFrameRate;
        return this;
    }

    public IWavInputBuilderFinalStage WithOffset(string offset)
    {
        _offset = offset;
        return this;
    }

    public IWavInputBuilderFinalStage WithFirstFrameOfAction(string firstFrameOfAction)
    {
        _firstFrameOfAction = firstFrameOfAction;
        return this;
    }

    public WavInput Build()
    {
        return new WavInput
        {
            FilePath = _filePath,
            TimeCodeFrameRate = _timeCodeFrameRate,
            Offset = _offset,
            FirstFrameOfAction = _firstFrameOfAction
        };
    }

    public IWavInputBuilderFinalStage WithFilePath(string filePath)
    {
        _filePath = filePath;
        return this;
    }
}
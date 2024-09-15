using MediaBedrock.Dolby.Jobs.Models.Filters;

namespace MediaBedrock.Dolby.Jobs.Models.Inputs;

public sealed record AtmosMezzanineInput : IJobInput
{
    public required string FilePath { get; init; }
    public required TimeCodeFrameRate TimeCodeFrameRate { get; init; } = TimeCodeFrameRate.NotIndicated;
    public required string Offset { get; init; }
    public required string FirstFrameOfAction { get; init; }

    public static IAtmosMezzanineInputBuilderInitialStage CreateBuilder()
    {
        return new AtmosMezzanineInputBuilder();
    }
}

public interface IAtmosMezzanineInputBuilderInitialStage
{
    IAtmosMezzanineInputBuilderFinalStage WithFilePath(string filePath);
}

public interface IAtmosMezzanineInputBuilderFinalStage
{
    IAtmosMezzanineInputBuilderFinalStage WithTimeCodeFrameRate(TimeCodeFrameRate timeCodeFrameRate);
    IAtmosMezzanineInputBuilderFinalStage WithOffset(string offset);
    IAtmosMezzanineInputBuilderFinalStage WithFirstFrameOfAction(string firstFrameOfAction);
    AtmosMezzanineInput Build();
}

public sealed class AtmosMezzanineInputBuilder : IAtmosMezzanineInputBuilderInitialStage,
    IAtmosMezzanineInputBuilderFinalStage
{
    private string _filePath = null!;
    private string _firstFrameOfAction = "auto";
    private string _offset = "auto";
    private TimeCodeFrameRate _timeCodeFrameRate = TimeCodeFrameRate.NotIndicated;

    internal AtmosMezzanineInputBuilder()
    {
    }

    public IAtmosMezzanineInputBuilderFinalStage WithTimeCodeFrameRate(TimeCodeFrameRate timeCodeFrameRate)
    {
        _timeCodeFrameRate = timeCodeFrameRate;
        return this;
    }

    public IAtmosMezzanineInputBuilderFinalStage WithOffset(string offset)
    {
        _offset = offset;
        return this;
    }

    public IAtmosMezzanineInputBuilderFinalStage WithFirstFrameOfAction(string firstFrameOfAction)
    {
        _firstFrameOfAction = firstFrameOfAction;
        return this;
    }

    public AtmosMezzanineInput Build()
    {
        return new AtmosMezzanineInput
        {
            FilePath = _filePath,
            TimeCodeFrameRate = _timeCodeFrameRate,
            Offset = _offset,
            FirstFrameOfAction = _firstFrameOfAction
        };
    }

    public IAtmosMezzanineInputBuilderFinalStage WithFilePath(string filePath)
    {
        _filePath = filePath;
        return this;
    }
}
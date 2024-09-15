using MediaBedrock.Dolby.Jobs.Models.Filters;

namespace MediaBedrock.Dolby.Jobs.Models.Inputs;

public sealed record AudioDefinitionModelInput : IJobInput
{
    public required string FilePath { get; init; }
    public required TimeCodeFrameRate TimeCodeFrameRate { get; init; } = TimeCodeFrameRate.NotIndicated;
    public required string Offset { get; init; }
    public required string FirstFrameOfAction { get; init; }

    public static IAudioDefinitionModelInputBuilderInitialStage CreateBuilder()
    {
        return new AudioDefinitionModelInputBuilder();
    }
}

public interface IAudioDefinitionModelInputBuilderInitialStage
{
    IAudioDefinitionModelInputBuilderFinalStage WithFilePath(string filePath);
}

public interface IAudioDefinitionModelInputBuilderFinalStage
{
    IAudioDefinitionModelInputBuilderFinalStage WithTimeCodeFrameRate(TimeCodeFrameRate timeCodeFrameRate);
    IAudioDefinitionModelInputBuilderFinalStage WithOffset(string offset);
    IAudioDefinitionModelInputBuilderFinalStage WithFirstFrameOfAction(string firstFrameOfAction);
    AudioDefinitionModelInput Build();
}

public sealed class AudioDefinitionModelInputBuilder :
    IAudioDefinitionModelInputBuilderInitialStage,
    IAudioDefinitionModelInputBuilderFinalStage
{
    private string _filePath = null!;
    private string _firstFrameOfAction = "auto";
    private string _offset = "auto";
    private TimeCodeFrameRate _timeCodeFrameRate = TimeCodeFrameRate.NotIndicated;

    internal AudioDefinitionModelInputBuilder()
    {
    }

    public IAudioDefinitionModelInputBuilderFinalStage WithTimeCodeFrameRate(TimeCodeFrameRate timeCodeFrameRate)
    {
        _timeCodeFrameRate = timeCodeFrameRate;
        return this;
    }

    public IAudioDefinitionModelInputBuilderFinalStage WithOffset(string offset)
    {
        _offset = offset;
        return this;
    }

    public IAudioDefinitionModelInputBuilderFinalStage WithFirstFrameOfAction(string firstFrameOfAction)
    {
        _firstFrameOfAction = firstFrameOfAction;
        return this;
    }

    public AudioDefinitionModelInput Build()
    {
        return new AudioDefinitionModelInput
        {
            FilePath = _filePath,
            TimeCodeFrameRate = _timeCodeFrameRate,
            Offset = _offset,
            FirstFrameOfAction = _firstFrameOfAction
        };
    }

    public IAudioDefinitionModelInputBuilderFinalStage WithFilePath(string filePath)
    {
        _filePath = filePath;
        return this;
    }
}
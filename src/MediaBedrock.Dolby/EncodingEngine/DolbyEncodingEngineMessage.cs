namespace MediaBedrock.Dolby.EncodingEngine;

public sealed record DolbyEncodingEngineMessage
{
    internal DolbyEncodingEngineMessage(string stage)
    {
        Stage = stage;
    }

    internal DolbyEncodingEngineMessage()
    {
    }

    public string Stage { get; init; } = string.Empty;
    public string StageName { get; init; } = string.Empty;
    public string Step { get; init; } = string.Empty;
    public double StageProgress { get; init; }
    public double OverallProgress { get; init; }
}
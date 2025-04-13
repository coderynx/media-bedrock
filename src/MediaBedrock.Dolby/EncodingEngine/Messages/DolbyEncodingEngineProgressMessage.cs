namespace MediaBedrock.Dolby.EncodingEngine.Messages;

public sealed record DolbyEncodingEngineProgressMessage : DolbyEncodingEngineMessage
{
    internal DolbyEncodingEngineProgressMessage(
        string message,
        string stage,
        string stageName,
        string step,
        double stageProgress,
        double overallProgress) : base(message)
    {
        if (string.IsNullOrWhiteSpace(stage))
        {
            throw new ArgumentException("Stage cannot be null or whitespace.", nameof(stage));
        }

        if (string.IsNullOrWhiteSpace(stageName))
        {
            throw new ArgumentException("StageName cannot be null or whitespace.", nameof(stageName));
        }

        if (string.IsNullOrWhiteSpace(step))
        {
            throw new ArgumentException("Step cannot be null or whitespace.", nameof(step));
        }

        if (stageProgress is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(stageProgress), "StageProgress must be between 0 and 100.");
        }

        if (overallProgress is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                paramName: nameof(overallProgress),
                message: "OverallProgress must be between 0 and 100.");
        }

        Message = message;
        Stage = stage;
        StageName = stageName;
        Step = step;
        StageProgress = stageProgress;
        OverallProgress = overallProgress;
    }

    public string Stage { get; init; }
    public string StageName { get; init; } = string.Empty;
    public string Step { get; init; } = string.Empty;
    public double StageProgress { get; init; }
    public double OverallProgress { get; init; }
}
using System.Text.RegularExpressions;

namespace MediaBedrock.Dolby.EncodingEngine;

internal sealed partial record DolbyEncodingEngineLogLine
{
    private DolbyEncodingEngineLogLine()
    {
    }

    public required DolbyEncodingEngineLogLevel Level { get; init; }
    public required string Category { get; init; }
    public required string Message { get; init; }

    public static DolbyEncodingEngineLogLine? Create(string line)
    {
        var match = LogRegex().Match(line);
        if (!match.Success)
        {
            return null;
        }

        return new DolbyEncodingEngineLogLine
        {
            Level = new DolbyEncodingEngineLogLevel(match.Groups["logLevel"].Value),
            Category = match.Groups["category"].Value,
            Message = match.Groups["message"].Value
        };
    }

    [GeneratedRegex(@"\[(?<timestamp>[^\]]+)\] (?<logLevel>\w+): (?<category>\w+): (?<message>.+)")]
    private static partial Regex LogRegex();
}
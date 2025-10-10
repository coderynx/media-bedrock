using System.Globalization;
using System.Text.RegularExpressions;

namespace MediaBedrock.Dolby.Encoding.Messages;

internal static partial class DolbyEncodingEngineMessageFactory
{
    public static DolbyEncodingEngineMessage Create(DolbyEncodingEngineLogLine logLine)
    {
        if (logLine.Level.Equals(DolbyEncodingEngineLogLevel.Error))
        {
            return new DolbyEncodingEngineErrorMessage(logLine.Message);
        }

        var match = ProgressRegex().Match(logLine.Message);
        if (match.Success)
        {
            return new DolbyEncodingEngineProgressMessage(
                message: logLine.Message,
                stage: match.Groups["Stage"].Value,
                stageName: match.Groups["StageName"].Value,
                step: match.Groups["Step"].Value,
                stageProgress: double.Parse(match.Groups["StageProgress"].Value, CultureInfo.InvariantCulture),
                overallProgress: double.Parse(match.Groups["OverallProgress"].Value, CultureInfo.InvariantCulture));
        }

        return new DolbyEncodingEngineInfoMessage(logLine.Message);
    }

    [GeneratedRegex(
        @"Stage:\s*(?<Stage>[^,]+),\s*Stage name:\s*(?<StageName>[^,]+),\s*Step:\s*(?<Step>[^,]+),\s*Stage progress:\s*(?<StageProgress>[\d.]+),\s*Overall progress:\s*(?<OverallProgress>[\d.]+)\b")]
    private static partial Regex ProgressRegex();
}
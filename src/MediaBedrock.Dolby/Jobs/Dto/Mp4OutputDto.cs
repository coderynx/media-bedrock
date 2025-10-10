using System.Text.Json.Serialization;

namespace MediaBedrock.Dolby.Jobs.Dto;

internal enum Mp4OutputFormatDto
{
    [JsonPropertyName("mp4")] Mp4,
    [JsonPropertyName("dash")] Dash
}

internal sealed record Mp4OutputDto
{
    [JsonPropertyName("-version")] public string Version { get; init; } = "1";
    public required Mp4OutputFormatDto OutputFormat { get; init; } = Mp4OutputFormatDto.Mp4;
    public string OverrideFrameRate { get; init; } = "no";
    public required string FileName { get; init; }
    public required StorageDto Storage { get; init; }
    public PluginDto Plugin { get; init; } = new();
}
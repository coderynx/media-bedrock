using System.Text.Json.Serialization;

namespace MediaBedrock.Dolby.Jobs.Dto;

internal sealed record Mp4OutputDto
{
    [JsonPropertyName("-version")] public string Version { get; init; } = "1";
    public required string OutputFormat { get; init; }
    public string OverrideFrameRate { get; init; } = "no";
    public required string FileName { get; init; }
    public required StorageDto Storage { get; init; }
    public PluginDto Plugin { get; init; } = new();
}
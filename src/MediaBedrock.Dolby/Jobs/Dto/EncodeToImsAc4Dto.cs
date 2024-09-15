using System.Text.Json.Serialization;

namespace MediaBedrock.Dolby.Jobs.Dto;

internal sealed record EncodeToImsAc4Dto
{
    [JsonPropertyName("-version")] public string Version { get; init; } = "1";
    public string TimecodeFrameRate { get; init; } = "not_indicated";
    public string Start { get; init; } = "first_frame_of_action";
    public string End { get; init; } = "end_of_file";
    public string TimeBase { get; init; } = "file_position";
    public string PrependSilenceDuration { get; init; } = "0";
    public string AppendSilenceDuration { get; init; } = "0";
    public LoudnessDto Loudness { get; init; } = new();
    public string DataRate { get; init; } = "256";
    public string Ac4FrameRate { get; init; } = "native";
    public bool ImsLegacyPresentation { get; init; }
    public int IframeInterval { get; init; }
    public string? Language { get; init; }
    public string EncodingProfile { get; init; } = "ims";
    public EncodeToImsAc4DrcDto Drc { get; init; } = new();
}

internal sealed record EncodeToImsAc4DrcDto
{
    public string DdpDrcProfile { get; init; } = "none";
    public string FlatPanelDrcProfile { get; init; } = "none";
    public string HomeTheatreDrcProfile { get; init; } = "none";
    public string PortableHpDrcProfile { get; init; } = "none";
    public string PortableSpkrDrcProfile { get; init; } = "none";
}
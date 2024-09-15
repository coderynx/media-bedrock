using System.Text.Json.Serialization;

namespace MediaBedrock.Dolby.Jobs.Dto;

internal record EncodeToAtmosDdpDto
{
    [JsonPropertyName("-version")] public string Version { get; init; } = "1";
    public LoudnessDto Loudness { get; init; } = new();
    public string DataRate { get; init; } = "448";
    public string TimecodeFrameRate { get; init; } = "not_indicated";
    public string Start { get; init; } = "first_frame_of_action";
    public string End { get; init; } = "end_of_file";
    public string TimeBase { get; init; } = "file_position";
    public string PrependSilenceDuration { get; init; } = "0.0";
    public string AppendSilenceDuration { get; init; } = "0.0";
    public DrcDto Drc { get; init; } = new();
    public DownmixDto Downmix { get; init; } = new();
    public CustomTrimsDto CustomTrims { get; init; } = new();
}

internal record LoudnessDto
{
    public MeasureOnly MeasureOnly { get; init; } = new();
}

internal record MeasureOnly
{
    public string MeteringMode { get; init; } = "1770-4";
    public string DialogueIntelligence { get; init; } = "false";
    public string SpeechThreshold { get; init; } = "15";
}

internal record DrcDto
{
    public string LineModeDrcProfile { get; init; } = "music_light";
    public string RfModeDrcProfile { get; init; } = "music_light";
}

internal record DownmixDto
{
    public string LoroCenterMixLevel { get; init; } = "-3";
    public string LoroSurroundMixLevel { get; init; } = "-3";
    public string LtrtCenterMixLevel { get; init; } = "-3";
    public string LtrtSurroundMixLevel { get; init; } = "-3";
    public string PreferredDownmixMode { get; init; } = "loro";
}

internal record CustomTrimsDto
{
    public string SurroundTrim51 { get; init; } = "auto";
    public string HeightTrim51 { get; init; } = "auto";
}
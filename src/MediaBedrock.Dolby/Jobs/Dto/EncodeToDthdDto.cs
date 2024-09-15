using System.Text.Json.Serialization;

namespace MediaBedrock.Dolby.Jobs.Dto;

internal sealed record EncodeToDthdDto
{
    [JsonPropertyName("-version")] public string Version { get; init; } = "1";
    public LoudnessMeasurementDto LoudnessMeasurement { get; init; } = new();
    public string TimecodeFrameRate { get; init; } = "not_indicated";
    public string Start { get; init; } = "first_frame_of_action";
    public string End { get; init; } = "end_of_file";
    public string TimeBase { get; init; } = "file_position";
    public string PrependSilenceDuration { get; init; } = "0";
    public string AppendSilenceDuration { get; init; } = "0";
    public AtmosPresentationDto AtmosPresentation { get; init; } = new();
    [JsonPropertyName("presentation_8ch")] public Presentation8ChDto Presentation8Ch { get; init; } = new();
    [JsonPropertyName("presentation_6ch")] public Presentation6ChDto Presentation6Ch { get; init; } = new();
    [JsonPropertyName("presentation_2ch")] public Presentation2ChDto Presentation2Ch { get; init; } = new();
    public bool OptimizeDataRate { get; init; }
    public EmbeddedTimecodes EmbeddedTimecodes { get; init; } = new();
    public string LogFormat { get; init; } = "txt";
}

internal sealed record LoudnessMeasurementDto
{
    public string MeteringMode { get; init; } = "1770-4";
    public bool DialogueIntelligence { get; init; } = false;
    public string SpeechThreshold { get; init; } = "15";
}

internal sealed record AtmosPresentationDto
{
    public string DrcProfile { get; init; } = "film_standard";
    public string SpatialClusters { get; init; } = "12";
    public bool LegacyAuthoringCompatibility { get; init; } = true;
}

internal sealed record Presentation8ChDto
{
    public string DrcProfile { get; init; } = "film_standard";

    [JsonPropertyName("surround_3db_attenuation")]
    public bool Surround3DbAttenuation { get; init; } = true;
}

internal sealed record Presentation6ChDto
{
    public string DrcProfile { get; init; } = "film_standard";

    [JsonPropertyName("surround_3db_attenuation")]
    public bool Surround3DbAttenuation { get; init; } = true;
}

public record Presentation2ChDto
{
    public string DrcProfile { get; init; } = "none";
    public bool DrcDefaultOn { get; init; }
    public string Format { get; init; } = "stereo";
}

internal sealed record EmbeddedTimecodes
{
    public string StartingTimecode { get; init; } = "off";
    public string FrameRate { get; init; } = "auto";
}
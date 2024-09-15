namespace MediaBedrock.Dolby.Jobs.Dto;

internal sealed record DdpDecodeDto
{
    public int Threads { get; init; } = 1;
    public string DownmixConfig { get; init; } = "off";
    public string StereoMode { get; init; } = "auto";
    public string Drc { get; init; } = "none";
    public string TimecodeFrameRate { get; init; } = "not_indicated";
    public string Start { get; init; } = "0:0:0.0";
    public string End { get; init; } = "end_of_file";

    public LoudnessDto Loudness { get; init; } = new()
    {
        MeasureOnly = new MeasureOnly
        {
            MeteringMode = "1770-3"
        }
    };
}
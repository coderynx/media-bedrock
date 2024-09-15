namespace MediaBedrock.Dolby.Jobs.Models.Filters;

public sealed record DolbyTrueHdStereoPresentation
{
    public DrcProfile DrcProfile { get; init; } = DrcProfile.FilmStandard;
    public bool DrcDefaultOn { get; init; } = false;
    public DolbyTrueHdStereoFormat Format { get; init; } = DolbyTrueHdStereoFormat.Stereo;
}
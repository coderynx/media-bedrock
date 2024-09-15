namespace MediaBedrock.Dolby.Jobs.Models.Filters;

public sealed record DolbyTrueHdSixChannelPresentation
{
    public DrcProfile DrcProfile { get; init; } = DrcProfile.FilmStandard;
    public bool Surround3DbAttenuation { get; init; } = true;
}
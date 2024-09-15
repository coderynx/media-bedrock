namespace MediaBedrock.Dolby.Jobs.Models.Filters;

public sealed record DolbyTrueHdEightChannelPresentation
{
    public DrcProfile DrcProfile { get; init; } = DrcProfile.FilmStandard;
    public bool Surround3DbAttenuation { get; init; } = true;
}
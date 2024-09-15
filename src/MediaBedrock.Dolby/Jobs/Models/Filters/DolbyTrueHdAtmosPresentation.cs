namespace MediaBedrock.Dolby.Jobs.Models.Filters;

public sealed record DolbyTrueHdAtmosPresentation
{
    public DrcProfile DrcProfile { get; init; } = DrcProfile.FilmStandard;
    public SpatialClusters SpatialClusters { get; init; } = SpatialClusters.Twelve;
    public bool LegacyAuthoringCompatibility { get; init; } = true;
}
namespace MediaBedrock.Dolby.Jobs.Models.Filters;

public enum SpatialClusters
{
    Twelve = 12,
    Fourteen = 14,
    Sixteen = 16
}

public enum DolbyTrueHdStereoFormat
{
    Stereo,
    DolbySurroundEncoded,
    DolbyHeadphoneEncoded
}

public sealed record EncodeToDolbyTrueHd : IJobFilter
{
    public TimeCodeFrameRate TimeCodeFrameRate { get; init; } = TimeCodeFrameRate.NotIndicated;
    public DolbyTrueHdAtmosPresentation AtmosPresentation { get; init; } = new();
    public DolbyTrueHdEightChannelPresentation EightChannelPresentation { get; init; } = new();
    public DolbyTrueHdSixChannelPresentation SixChannelPresentation { get; init; } = new();
    public DolbyTrueHdStereoPresentation StereoPresentation { get; init; } = new();
    public bool OptimizeDataRate { get; init; }

    public static EncodeToDolbyTrueHdBuilder CreateBuilder()
    {
        return new EncodeToDolbyTrueHdBuilder();
    }
}

public sealed class EncodeToDolbyTrueHdBuilder
{
    private DolbyTrueHdAtmosPresentation _atmosPresentation = new();
    private DolbyTrueHdEightChannelPresentation _eightChannelPresentation = new();
    private bool _optimizeDataRate;
    private DolbyTrueHdSixChannelPresentation _sixChannelPresentation = new();
    private DolbyTrueHdStereoPresentation _stereoPresentation = new();

    private TimeCodeFrameRate _timeCodeFrameRate = TimeCodeFrameRate.NotIndicated;

    internal EncodeToDolbyTrueHdBuilder()
    {
    }

    public EncodeToDolbyTrueHdBuilder WithTimeCodeFrameRate(TimeCodeFrameRate timeCodeFrameRate)
    {
        _timeCodeFrameRate = timeCodeFrameRate;
        return this;
    }

    public EncodeToDolbyTrueHdBuilder WithAtmosPresentation(
        DolbyTrueHdAtmosPresentation dolbyTrueHdAtmosPresentation)
    {
        _atmosPresentation = dolbyTrueHdAtmosPresentation;
        return this;
    }

    public EncodeToDolbyTrueHdBuilder WithEightChannelPresentation(
        DolbyTrueHdEightChannelPresentation dolbyTrueHdEightChannelPresentation)
    {
        _eightChannelPresentation = dolbyTrueHdEightChannelPresentation;
        return this;
    }

    public EncodeToDolbyTrueHdBuilder WithSixChannelPresentation(
        DolbyTrueHdSixChannelPresentation dolbyTrueHdSixChannelPresentation)
    {
        _sixChannelPresentation = dolbyTrueHdSixChannelPresentation;
        return this;
    }

    public EncodeToDolbyTrueHdBuilder WithStereoPresentation(
        DolbyTrueHdStereoPresentation dolbyTrueHdStereoPresentation)
    {
        _stereoPresentation = dolbyTrueHdStereoPresentation;
        return this;
    }

    public EncodeToDolbyTrueHdBuilder WithOptimizeDataRate(bool optimizeDataRate)
    {
        _optimizeDataRate = optimizeDataRate;
        return this;
    }

    public EncodeToDolbyTrueHd Build()
    {
        return new EncodeToDolbyTrueHd
        {
            TimeCodeFrameRate = _timeCodeFrameRate,
            AtmosPresentation = _atmosPresentation,
            EightChannelPresentation = _eightChannelPresentation,
            SixChannelPresentation = _sixChannelPresentation,
            StereoPresentation = _stereoPresentation,
            OptimizeDataRate = _optimizeDataRate
        };
    }
}
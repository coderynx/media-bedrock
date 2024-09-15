namespace MediaBedrock.Dolby.Jobs.Models.Filters;

public sealed record EncodeToAtmosDolbyDigitalPlus : IJobFilter
{
    public TimeCodeFrameRate TimeCodeFrameRate { get; init; } = TimeCodeFrameRate.NotIndicated;
    public DrcProfile LineModeDrcProfile { get; init; } = DrcProfile.None;
    public DrcProfile RfModeDrcProfile { get; init; } = DrcProfile.None;

    public static EncodeToAtmosDolbyDigitalPlusBuilder CreateBuilder()
    {
        return new EncodeToAtmosDolbyDigitalPlusBuilder();
    }
}

public sealed class EncodeToAtmosDolbyDigitalPlusBuilder
{
    private DrcProfile _lineModeDrcProfile = DrcProfile.None;
    private DrcProfile _rfModeDrcProfile = DrcProfile.None;

    private TimeCodeFrameRate _timeCodeFrameRate = TimeCodeFrameRate.NotIndicated;

    internal EncodeToAtmosDolbyDigitalPlusBuilder()
    {
    }

    public EncodeToAtmosDolbyDigitalPlusBuilder WithTimeCodeFrameRate(TimeCodeFrameRate timeCodeFrameRate)
    {
        _timeCodeFrameRate = timeCodeFrameRate;
        return this;
    }

    public EncodeToAtmosDolbyDigitalPlusBuilder WithLineModeDrcProfile(DrcProfile lineModeDrcProfile)
    {
        _lineModeDrcProfile = lineModeDrcProfile;
        return this;
    }

    public EncodeToAtmosDolbyDigitalPlusBuilder WithRfModeDrcProfile(DrcProfile rfModeDrcProfile)
    {
        _rfModeDrcProfile = rfModeDrcProfile;
        return this;
    }

    public EncodeToAtmosDolbyDigitalPlus Build()
    {
        return new EncodeToAtmosDolbyDigitalPlus
        {
            TimeCodeFrameRate = _timeCodeFrameRate,
            LineModeDrcProfile = _lineModeDrcProfile,
            RfModeDrcProfile = _rfModeDrcProfile
        };
    }
}
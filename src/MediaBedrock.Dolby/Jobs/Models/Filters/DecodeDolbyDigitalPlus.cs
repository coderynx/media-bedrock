namespace MediaBedrock.Dolby.Jobs.Models.Filters;

public enum DolbyDigitalPlusDownMixConfiguration
{
    Off,
    Mono,
    Stereo,
    FivePointOne
}

public enum DolbyDigitalPlusStereoMode
{
    Auto,
    LoRo,
    LtRt
}

public enum DolbyDigitalPlusDrcProfile
{
    None,
    Line,
    Rf
}

public sealed record DecodeDolbyDigitalPlus : IJobFilter
{
    public int Threads { get; init; } = 1;
    public TimeCodeFrameRate TimeCodeFrameRate { get; init; } = TimeCodeFrameRate.NotIndicated;

    public DolbyDigitalPlusDownMixConfiguration DownmixConfiguration { get; init; } =
        DolbyDigitalPlusDownMixConfiguration.Off;

    public DolbyDigitalPlusStereoMode StereoMode { get; init; } = DolbyDigitalPlusStereoMode.Auto;
    public DolbyDigitalPlusDrcProfile Drc { get; init; } = DolbyDigitalPlusDrcProfile.None;

    public static DecodeDolbyDigitalPlusBuilder CreateBuilder()
    {
        return new DecodeDolbyDigitalPlusBuilder();
    }
}

public sealed class DecodeDolbyDigitalPlusBuilder
{
    private DolbyDigitalPlusDownMixConfiguration _downmixConfiguration = DolbyDigitalPlusDownMixConfiguration.Off;
    private DolbyDigitalPlusDrcProfile _drc = DolbyDigitalPlusDrcProfile.None;
    private DolbyDigitalPlusStereoMode _stereoMode = DolbyDigitalPlusStereoMode.Auto;

    private int _threads = 1;
    private TimeCodeFrameRate _timeCodeFrameRate = TimeCodeFrameRate.NotIndicated;

    internal DecodeDolbyDigitalPlusBuilder()
    {
    }

    public DecodeDolbyDigitalPlusBuilder WithThreads(int threads)
    {
        _threads = threads;
        return this;
    }

    public DecodeDolbyDigitalPlusBuilder WithTimeCodeFrameRate(TimeCodeFrameRate timeCodeFrameRate)
    {
        _timeCodeFrameRate = timeCodeFrameRate;
        return this;
    }

    public DecodeDolbyDigitalPlusBuilder WithDownmixConfiguration(
        DolbyDigitalPlusDownMixConfiguration downmixConfiguration)
    {
        _downmixConfiguration = downmixConfiguration;
        return this;
    }

    public DecodeDolbyDigitalPlusBuilder WithStereoMode(DolbyDigitalPlusStereoMode stereoMode)
    {
        _stereoMode = stereoMode;
        return this;
    }

    public DecodeDolbyDigitalPlusBuilder WithDrc(DolbyDigitalPlusDrcProfile drc)
    {
        _drc = drc;
        return this;
    }

    public DecodeDolbyDigitalPlus Build()
    {
        return new DecodeDolbyDigitalPlus
        {
            Threads = _threads,
            TimeCodeFrameRate = _timeCodeFrameRate,
            DownmixConfiguration = _downmixConfiguration,
            StereoMode = _stereoMode,
            Drc = _drc
        };
    }
}
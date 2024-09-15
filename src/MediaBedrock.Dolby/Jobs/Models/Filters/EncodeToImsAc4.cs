namespace MediaBedrock.Dolby.Jobs.Models.Filters;

public enum Ac4FrameRate
{
    Native,
    TwentyThreeNineSevenSix,
    TwentyFour,
    TwentyFive,
    TwentyNineNineSeven
}

public enum Ac4DataRate
{
    SixtyFour,
    SeventyTwo,
    OneHundredTwelve,
    OneHundredFortyFour,
    TwoHundredFiftySix,
    ThreeHundredTwenty
}

public enum Ac4EncodingProfile
{
    Ims,
    ImsMusic
}

public sealed record EncodeToImsAc4 : IJobFilter
{
    public TimeCodeFrameRate TimeCodeFrameRate { get; init; } = TimeCodeFrameRate.NotIndicated;
    public Ac4DataRate DataRate { get; init; } = Ac4DataRate.TwoHundredFiftySix;
    public Ac4FrameRate Ac4FrameRate { get; init; } = Ac4FrameRate.Native;
    public bool ImsLegacyPresentation { get; init; }
    public int IframeInterval { get; init; }
    public string? Language { get; init; }
    public Ac4EncodingProfile EncodingProfile { get; init; } = Ac4EncodingProfile.Ims;
    public DrcProfile DolbyDigitalPlusDrcProfile { get; init; } = DrcProfile.None;
    public DrcProfile FlatPanelDrcProfile { get; init; } = DrcProfile.None;
    public DrcProfile HomeTheatreDrcProfile { get; init; } = DrcProfile.None;
    public DrcProfile PortableHpDrcProfile { get; init; } = DrcProfile.None;
    public DrcProfile PortableSpkrDrcProfile { get; init; } = DrcProfile.None;

    public static EncodeToImsAc4Builder CreateBuilder()
    {
        return new EncodeToImsAc4Builder();
    }
}

public sealed class EncodeToImsAc4Builder
{
    private Ac4FrameRate _ac4FrameRate = Ac4FrameRate.Native;
    private Ac4DataRate _dataRate = Ac4DataRate.TwoHundredFiftySix;
    private DrcProfile _dolbyDigitalPlusDrcProfile = DrcProfile.None;
    private Ac4EncodingProfile _encodingProfile = Ac4EncodingProfile.Ims;
    private DrcProfile _flatPanelDrcProfile = DrcProfile.None;
    private DrcProfile _homeTheatreDrcProfile = DrcProfile.None;
    private int _iframeInterval;
    private bool _imsLegacyPresentation;
    private string? _language;
    private DrcProfile _portableHpDrcProfile = DrcProfile.None;
    private DrcProfile _portableSpkrDrcProfile = DrcProfile.None;

    private TimeCodeFrameRate _timeCodeFrameRate = TimeCodeFrameRate.NotIndicated;

    internal EncodeToImsAc4Builder()
    {
    }

    public EncodeToImsAc4Builder WithTimeCodeFrameRate(TimeCodeFrameRate timeCodeFrameRate)
    {
        _timeCodeFrameRate = timeCodeFrameRate;
        return this;
    }

    public EncodeToImsAc4Builder WithDataRate(Ac4DataRate dataRate)
    {
        _dataRate = dataRate;
        return this;
    }

    public EncodeToImsAc4Builder WithAc4FrameRate(Ac4FrameRate ac4FrameRate)
    {
        _ac4FrameRate = ac4FrameRate;
        return this;
    }

    public EncodeToImsAc4Builder WithImsLegacyPresentation(bool imsLegacyPresentation)
    {
        _imsLegacyPresentation = imsLegacyPresentation;
        return this;
    }

    public EncodeToImsAc4Builder WithIframeInterval(int iframeInterval)
    {
        _iframeInterval = iframeInterval;
        return this;
    }

    public EncodeToImsAc4Builder WithLanguage(string? language)
    {
        _language = language;
        return this;
    }

    public EncodeToImsAc4Builder WithEncodingProfile(Ac4EncodingProfile encodingProfile)
    {
        _encodingProfile = encodingProfile;
        return this;
    }

    public EncodeToImsAc4Builder WithDolbyDigitalPlusDrcProfile(DrcProfile dolbyDigitalPlusDrcProfile)
    {
        _dolbyDigitalPlusDrcProfile = dolbyDigitalPlusDrcProfile;
        return this;
    }

    public EncodeToImsAc4Builder WithFlatPanelDrcProfile(DrcProfile flatPanelDrcProfile)
    {
        _flatPanelDrcProfile = flatPanelDrcProfile;
        return this;
    }

    public EncodeToImsAc4Builder WithHomeTheatreDrcProfile(DrcProfile homeTheatreDrcProfile)
    {
        _homeTheatreDrcProfile = homeTheatreDrcProfile;
        return this;
    }

    public EncodeToImsAc4Builder WithPortableHpDrcProfile(DrcProfile portableHpDrcProfile)
    {
        _portableHpDrcProfile = portableHpDrcProfile;
        return this;
    }

    public EncodeToImsAc4Builder WithPortableSpkrDrcProfile(DrcProfile portableSpkrDrcProfile)
    {
        _portableSpkrDrcProfile = portableSpkrDrcProfile;
        return this;
    }

    public EncodeToImsAc4 Build()
    {
        return new EncodeToImsAc4
        {
            TimeCodeFrameRate = _timeCodeFrameRate,
            DataRate = _dataRate,
            Ac4FrameRate = _ac4FrameRate,
            ImsLegacyPresentation = _imsLegacyPresentation,
            IframeInterval = _iframeInterval,
            Language = _language,
            EncodingProfile = _encodingProfile,
            DolbyDigitalPlusDrcProfile = _dolbyDigitalPlusDrcProfile,
            FlatPanelDrcProfile = _flatPanelDrcProfile,
            HomeTheatreDrcProfile = _homeTheatreDrcProfile,
            PortableHpDrcProfile = _portableHpDrcProfile,
            PortableSpkrDrcProfile = _portableSpkrDrcProfile
        };
    }
}
using MediaBedrock.Dolby.Jobs.Dto;

namespace MediaBedrock.Dolby.Jobs.Models.Filters;

internal static class EncodeToImsAc4Extensions
{
    private static string ToDtoString(this Ac4FrameRate frameRate)
    {
        return frameRate switch
        {
            Ac4FrameRate.Native => "native",
            Ac4FrameRate.TwentyFour => "24",
            Ac4FrameRate.TwentyFive => "25",
            Ac4FrameRate.TwentyThreeNineSevenSix => "23.976",
            Ac4FrameRate.TwentyNineNineSeven => "29.97",
            _ => throw new ArgumentOutOfRangeException(nameof(frameRate), frameRate, null)
        };
    }

    private static string ToDtoString(this Ac4DataRate dataRate)
    {
        return dataRate switch
        {
            Ac4DataRate.SixtyFour => "64",
            Ac4DataRate.SeventyTwo => "72",
            Ac4DataRate.OneHundredTwelve => "112",
            Ac4DataRate.OneHundredFortyFour => "144",
            Ac4DataRate.TwoHundredFiftySix => "256",
            Ac4DataRate.ThreeHundredTwenty => "320",
            _ => throw new ArgumentOutOfRangeException(nameof(dataRate), dataRate, null)
        };
    }

    private static string ToDtoString(this Ac4EncodingProfile profile)
    {
        return profile switch
        {
            Ac4EncodingProfile.Ims => "ims",
            Ac4EncodingProfile.ImsMusic => "ims_music",
            _ => throw new ArgumentOutOfRangeException(nameof(profile), profile, null)
        };
    }

    internal static JobFilterDto ToDto(this EncodeToImsAc4 filter)
    {
        return new JobFilterDto
        {
            Audio = new AudioOutputDto
            {
                EncodeToImsAc4 = new EncodeToImsAc4Dto
                {
                    TimecodeFrameRate = filter.TimeCodeFrameRate.ToDtoString(),
                    DataRate = filter.DataRate.ToDtoString(),
                    Ac4FrameRate = filter.Ac4FrameRate.ToDtoString(),
                    ImsLegacyPresentation = false,
                    IframeInterval = filter.IframeInterval,
                    Language = filter.Language,
                    EncodingProfile = filter.EncodingProfile.ToDtoString(),
                    Drc = new EncodeToImsAc4DrcDto
                    {
                        DdpDrcProfile = filter.DolbyDigitalPlusDrcProfile.ToDtoString(),
                        FlatPanelDrcProfile = filter.FlatPanelDrcProfile.ToDtoString(),
                        HomeTheatreDrcProfile = filter.HomeTheatreDrcProfile.ToDtoString(),
                        PortableHpDrcProfile = filter.PortableHpDrcProfile.ToDtoString(),
                        PortableSpkrDrcProfile = filter.PortableSpkrDrcProfile.ToDtoString()
                    }
                }
            }
        };
    }
}
using MediaBedrock.Dolby.Jobs.Dto;

namespace MediaBedrock.Dolby.Jobs.Models.Filters;

internal static class DecodeDolbyDigitalPlusExtensions
{
    private static string ToDtoString(this DolbyDigitalPlusStereoMode stereoMode)
    {
        return stereoMode switch
        {
            DolbyDigitalPlusStereoMode.Auto => "auto",
            DolbyDigitalPlusStereoMode.LoRo => "loro",
            DolbyDigitalPlusStereoMode.LtRt => "ltrt",
            _ => throw new ArgumentOutOfRangeException(nameof(stereoMode), stereoMode, null)
        };
    }

    private static string ToDtoString(this DolbyDigitalPlusDownMixConfiguration downMixConfiguration)
    {
        return downMixConfiguration switch
        {
            DolbyDigitalPlusDownMixConfiguration.Off => "off",
            DolbyDigitalPlusDownMixConfiguration.Mono => "mono",
            DolbyDigitalPlusDownMixConfiguration.Stereo => "stereo",
            DolbyDigitalPlusDownMixConfiguration.FivePointOne => "5.1",
            _ => throw new ArgumentOutOfRangeException(nameof(downMixConfiguration), downMixConfiguration, null)
        };
    }

    private static string ToDtoString(this DolbyDigitalPlusDrcProfile drcProfile)
    {
        return drcProfile switch
        {
            DolbyDigitalPlusDrcProfile.None => "none",
            DolbyDigitalPlusDrcProfile.Line => "line",
            DolbyDigitalPlusDrcProfile.Rf => "rf",
            _ => throw new ArgumentOutOfRangeException(nameof(drcProfile), drcProfile, null)
        };
    }

    public static JobFilterDto ToDto(this DecodeDolbyDigitalPlus filter)
    {
        return new JobFilterDto
        {
            Audio = new AudioOutputDto
            {
                DdpDecode = new DdpDecodeDto
                {
                    Threads = filter.Threads,
                    DownmixConfig = filter.DownmixConfiguration.ToDtoString(),
                    StereoMode = filter.StereoMode.ToDtoString(),
                    Drc = filter.Drc.ToDtoString(),
                    TimecodeFrameRate = filter.TimeCodeFrameRate.ToDtoString()
                }
            }
        };
    }
}
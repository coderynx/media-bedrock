using MediaBedrock.Dolby.Jobs.Dto;

namespace MediaBedrock.Dolby.Jobs.Models.Filters;

internal static class EncodeDolbyTrueHdExtensions
{
    public static JobFilterDto ToDto(this EncodeToDolbyTrueHd filter)
    {
        return new JobFilterDto
        {
            Audio = new AudioOutputDto
            {
                EncodeToDthd = new EncodeToDthdDto
                {
                    TimecodeFrameRate = filter.TimeCodeFrameRate.ToDtoString(),
                    AtmosPresentation = filter.AtmosPresentation.ToDto(),
                    Presentation8Ch = filter.EightChannelPresentation.ToDto(),
                    Presentation6Ch = filter.SixChannelPresentation.ToDto(),
                    Presentation2Ch = filter.StereoPresentation.ToDto(),
                    OptimizeDataRate = filter.OptimizeDataRate
                }
            }
        };
    }

    private static string ToStringDto(this SpatialClusters clusters)
    {
        return clusters switch
        {
            SpatialClusters.Twelve => "12",
            SpatialClusters.Fourteen => "14",
            SpatialClusters.Sixteen => "16",
            _ => throw new ArgumentOutOfRangeException(nameof(clusters), clusters, null)
        };
    }

    private static string ToStringDto(this DolbyTrueHdStereoFormat format)
    {
        return format switch
        {
            DolbyTrueHdStereoFormat.Stereo => "stereo",
            DolbyTrueHdStereoFormat.DolbySurroundEncoded => "dolby_surround_encoded",
            DolbyTrueHdStereoFormat.DolbyHeadphoneEncoded => "dolby_headphone_encoded",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }

    private static AtmosPresentationDto ToDto(this DolbyTrueHdAtmosPresentation presentation)
    {
        return new AtmosPresentationDto
        {
            DrcProfile = presentation.DrcProfile.ToDtoString(),
            SpatialClusters = presentation.SpatialClusters.ToStringDto(),
            LegacyAuthoringCompatibility = presentation.LegacyAuthoringCompatibility
        };
    }

    private static Presentation8ChDto ToDto(this DolbyTrueHdEightChannelPresentation presentation)
    {
        return new Presentation8ChDto
        {
            DrcProfile = presentation.DrcProfile.ToDtoString(),
            Surround3DbAttenuation = presentation.Surround3DbAttenuation
        };
    }

    private static Presentation6ChDto ToDto(this DolbyTrueHdSixChannelPresentation presentation)
    {
        return new Presentation6ChDto
        {
            DrcProfile = presentation.DrcProfile.ToDtoString(),
            Surround3DbAttenuation = presentation.Surround3DbAttenuation
        };
    }

    private static Presentation2ChDto ToDto(this DolbyTrueHdStereoPresentation presentation)
    {
        return new Presentation2ChDto
        {
            DrcProfile = presentation.DrcProfile.ToDtoString(),
            DrcDefaultOn = presentation.DrcDefaultOn,
            Format = presentation.Format.ToStringDto()
        };
    }
}
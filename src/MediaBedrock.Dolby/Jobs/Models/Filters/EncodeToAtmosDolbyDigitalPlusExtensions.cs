using MediaBedrock.Dolby.Jobs.Dto;

namespace MediaBedrock.Dolby.Jobs.Models.Filters;

internal static class EncodeToAtmosDolbyDigitalPlusExtensions
{
    internal static JobFilterDto ToDto(this EncodeToAtmosDolbyDigitalPlus filter)
    {
        return new JobFilterDto
        {
            Audio = new AudioOutputDto
            {
                EncodeToAtmosDdp = new EncodeToAtmosDdpDto
                {
                    TimecodeFrameRate = filter.TimeCodeFrameRate.ToDtoString(),
                    Drc = new DrcDto
                    {
                        LineModeDrcProfile = filter.LineModeDrcProfile.ToDtoString(),
                        RfModeDrcProfile = filter.RfModeDrcProfile.ToDtoString()
                    },
                    Loudness = new LoudnessDto()
                }
            }
        };
    }
}
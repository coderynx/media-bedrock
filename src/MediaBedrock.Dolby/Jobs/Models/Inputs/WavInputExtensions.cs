using MediaBedrock.Dolby.Jobs.Dto;
using MediaBedrock.Dolby.Jobs.Models.Filters;

namespace MediaBedrock.Dolby.Jobs.Models.Inputs;

internal static class WavInputExtensions
{
    internal static JobInputDto ToDto(this WavInput input)
    {
        return new JobInputDto
        {
            Audio = new AudioInputDto
            {
                Wav = new WavInputDto
                {
                    FileName = Path.GetFileName(input.FilePath),
                    TimecodeFrameRate = input.TimeCodeFrameRate.ToDtoString(),
                    Offset = input.Offset,
                    Ffoa = input.FirstFrameOfAction,
                    Storage = new StorageDto
                    {
                        Local = new LocalDto
                        {
                            Path = Path.GetDirectoryName(input.FilePath)!
                        }
                    }
                }
            }
        };
    }
}
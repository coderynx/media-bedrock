using MediaBedrock.Dolby.Jobs.Dto;

namespace MediaBedrock.Dolby.Jobs.Models.Inputs;

internal static class Ec3InputExtensions
{
    internal static JobInputDto ToDto(this Ec3Input input)
    {
        return new JobInputDto
        {
            Audio = new AudioInputDto
            {
                Ec3 = new Ec3InputDto
                {
                    FileName = Path.GetFileName(input.FilePath),
                    StreamType = input.IsAtmos ? "atmos" : "non_atmos",
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
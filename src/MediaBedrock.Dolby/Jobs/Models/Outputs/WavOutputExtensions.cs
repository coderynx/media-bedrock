using MediaBedrock.Dolby.Jobs.Dto;

namespace MediaBedrock.Dolby.Jobs.Models.Outputs;

internal static class WavOutputExtensions
{
    internal static WavOutputDto ToDto(this WavOutput output)
    {
        return new WavOutputDto
        {
            FileName = Path.GetFileName(output.FilePath),
            Storage = new StorageDto
            {
                Local = new LocalDto
                {
                    Path = Path.GetDirectoryName(output.FilePath)!
                }
            }
        };
    }
}
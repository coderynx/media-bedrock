using MediaBedrock.Dolby.Jobs.Dto;

namespace MediaBedrock.Dolby.Jobs.Models.Outputs;

internal static class MlpOutputExtensions
{
    internal static MlpOutputDto ToDto(this MlpOutput output)
    {
        return new MlpOutputDto
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
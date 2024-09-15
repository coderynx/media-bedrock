using MediaBedrock.Dolby.Jobs.Dto;

namespace MediaBedrock.Dolby.Jobs.Models.Outputs;

public static class ManifestOutputExtensions
{
    internal static ManifestOutputDto ToDto(this ManifestOutput output)
    {
        return new ManifestOutputDto
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
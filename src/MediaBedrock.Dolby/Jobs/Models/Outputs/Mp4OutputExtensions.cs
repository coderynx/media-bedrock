using MediaBedrock.Dolby.Jobs.Dto;

namespace MediaBedrock.Dolby.Jobs.Models.Outputs;

internal static class Mp4OutputExtensions
{
    internal static Mp4OutputDto ToDto(this Mp4Output output)
    {
        return new Mp4OutputDto
        {
            FileName = Path.GetFileName(output.FilePath),
            OutputFormat = output.OutputAsDash
                ? "dash"
                : "mp4",
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
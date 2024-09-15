namespace MediaBedrock.Dolby.Jobs.Dto;

internal sealed record AtmosMezzanineInputDto
{
    public required string FileName { get; init; }
    public required string TimecodeFrameRate { get; init; }
    public string Offset { get; init; } = "auto";
    public string Ffoa { get; init; } = "auto";
    public required StorageDto Storage { get; init; }
}
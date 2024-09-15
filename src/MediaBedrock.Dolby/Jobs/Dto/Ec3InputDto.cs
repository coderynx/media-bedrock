namespace MediaBedrock.Dolby.Jobs.Dto;

internal sealed record Ec3InputDto
{
    public required string FileName { get; init; }
    public required string StreamType { get; init; }
    public required StorageDto Storage { get; init; }
}
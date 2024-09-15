namespace MediaBedrock.Dolby.Jobs.Dto;

internal sealed record ManifestOutputDto
{
    public required string FileName { get; init; }
    public required StorageDto Storage { get; init; }
}
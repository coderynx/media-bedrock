using System.ComponentModel.DataAnnotations;

namespace MediaBedrock.Dolby.Extensions.DependencyInjection;

public sealed record DolbyEncodingEngineSettings
{
    public const string SectionName = "Dolby:EncodingEngine";

    [Required] public required string DirectoryPath { get; set; }
    public bool UseWine { get; set; }
}
namespace MediaBedrock.Dolby.Jobs.Dto;

internal sealed record JobDefinitionDto
{
    public required JobConfigDto JobConfig { get; init; }
}

internal sealed record JobConfigDto
{
    public required JobInputDto Input { get; init; }
    public required JobFilterDto Filter { get; init; }
    public required JobOutputDto Output { get; init; }
    public required JobMiscellaneousDto Misc { get; init; }
}

internal sealed record JobInputDto
{
    public AudioInputDto? Audio { get; init; }
}

internal sealed record AudioInputDto
{
    public AdmBwfInputDto? Adm { get; init; }
    public AtmosMezzanineInputDto? AtmosMezz { get; init; }
    public Ec3InputDto? Ec3 { get; init; }
}

internal sealed record StorageDto
{
    public required LocalDto Local { get; init; }
}

internal sealed record LocalDto
{
    public required string Path { get; init; }
}

internal sealed record JobFilterDto
{
    public AudioOutputDto? Audio { get; init; } = null;
}

internal sealed record AudioOutputDto
{
    public EncodeToAtmosDdpDto? EncodeToAtmosDdp { get; init; }
    public EncodeToImsAc4Dto? EncodeToImsAc4 { get; init; }
    public EncodeToDthdDto? EncodeToDthd { get; init; }
    public DdpDecodeDto? DdpDecode { get; init; }
}

internal sealed record JobOutputDto
{
    public Mp4OutputDto? Mp4 { get; init; }
    public MlpOutputDto? Mlp { get; init; }
    public WavOutputDto? Wav { get; init; }
    public ManifestOutputDto? Manifest { get; init; }
}

internal record PluginDto
{
    public BaseDto Base { get; init; } = new();
}

internal record BaseDto;

internal record JobMiscellaneousDto
{
    public TempDir TempDir { get; init; } = new();
}

internal record TempDir
{
    public string CleanTemp { get; init; } = "true";
    public string Path { get; init; } = "temp";
}
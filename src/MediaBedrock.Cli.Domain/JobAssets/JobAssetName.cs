namespace MediaBedrock.Cli.Domain.JobAssets;

public sealed record JobAssetName(string Value)
{
    public override string ToString()
    {
        return Value;
    }
}
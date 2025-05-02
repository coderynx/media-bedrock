namespace MediaBedrock.Cli.Domain.JobAssets;

public sealed record JobAssetId
{
    public JobAssetId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException(JobAssetErrors.InvalidAssetIdCode, "Asset ID cannot be empty.");
        }

        Value = value;
    }

    public JobAssetId()
    {
        Value = Guid.CreateVersion7();
    }

    public Guid Value { get; init; }
}
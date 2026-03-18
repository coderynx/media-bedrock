namespace MediaBedrock.Controller.Domain.JobAssets;

public sealed record JobAssetId
{
    public JobAssetId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw JobAssetErrors.InvalidAssetId();
        }

        Value = value;
    }

    public JobAssetId()
    {
        Value = Guid.CreateVersion7();
    }

    public Guid Value { get; init; }
}
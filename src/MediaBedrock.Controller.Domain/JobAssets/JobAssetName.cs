using Coderynx.Functional.Results;

namespace MediaBedrock.Controller.Domain.JobAssets;

public sealed record JobAssetName
{
    public JobAssetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw JobAssetErrors.InvalidName(name);
        }

        Value = name;
    }

    public string Value { get; init; }

    public override string ToString()
    {
        return Value;
    }

    public static Result<JobAssetName> Create(string name)
    {
        return Result.TryCatch(() => new JobAssetName(name));
    }
}
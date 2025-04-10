using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Assets;

public static class JobAssetErrors
{
    public static Error AssetNotFound(string name)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: "JobAsset.AssetNotFound",
            Message: $"Asset '{name}' not found in the job assets pool.");
    }

    public static Error AssetNotAvailable(string name)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: "JobAsset.AssetNotAvailable",
            Message: $"Asset {name} not available in the job assets pool.");
    }
}
using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.JobAssets;

public static class JobAssetErrors
{
    public const string InvalidAssetIdCode = "JobAsset.InvalidAssetId";
    public const string NotFoundCode = "JobAsset.AssetNotFound";
    public const string NotAvailableCode = "JobAsset.AssetNotAvailable";
    public const string InvalidUriCode = "JobAsset.InvalidUri";
    public const string InvalidNameCode = "JobAsset.InvalidName";
    public const string FailedToRetrieveMediaInformationCode = "Asset.FailedToRetrieveInformation";

    public static Error NotFound(JobAssetName name)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: NotFoundCode,
            Message: $"Asset '{name}' not found in the job assets pool.");
    }

    public static Error NotAvailable(JobAssetName name)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: NotAvailableCode,
            Message: $"Asset {name} not available in the job assets pool.");
    }

    public static Error InvalidUri(string uri)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidUriCode,
            Message: $"The asset URI '{uri}' is invalid. Please check the URI and try again.");
    }

    public static Error InvalidName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidNameCode,
            Message: $"The asset name '{name}' is invalid. Please check the name and try again.");
    }

    public static Error FailedToRetrieveMediaInformation(string uri)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: FailedToRetrieveMediaInformationCode,
            Message: $"Failed to retrieve asset media information for {uri}");
    }
}
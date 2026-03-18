using Coderynx.Functional.Results.Errors;

namespace MediaBedrock.Controller.Domain.JobAssets;

public static class JobAssetErrorCodes
{
    public const string InvalidAssetId = "JobAsset.InvalidAssetId";
    public const string NotFound = "JobAsset.AssetNotFound";
    public const string NotAvailable = "JobAsset.AssetNotAvailable";
    public const string InvalidUri = "JobAsset.InvalidUri";
    public const string InvalidName = "JobAsset.InvalidName";
    public const string FailedToRetrieveMediaInformation = "Asset.FailedToRetrieveInformation";
}

public static class JobAssetErrors
{
    public static Error NotFound(JobAssetId id)
    {
        return Error.NotFound(
            code: JobAssetErrorCodes.NotFound,
            message: $"Asset '{id}' not found in the job assets pool.");
    }
    
    public static Error NotFound(JobAssetName name)
    {
        return Error.NotFound(
            code: JobAssetErrorCodes.NotFound,
            message: $"Asset '{name}' not found in the job assets pool.");
    }

    public static Error NotAvailable(JobAssetName name)
    {
        return Error.NotFound(
            code: JobAssetErrorCodes.NotAvailable,
            message: $"Asset '{name}' not available in the job assets pool.");       
    }

    public static Error InvalidUri(string uri)
    {
        return Error.InvalidInput(
            code: JobAssetErrorCodes.InvalidUri,
            message: $"The asset URI '{uri}' is invalid. Please check the URI and try again.");
    }

    public static Error InvalidName(string name)
    {
        return Error.InvalidInput(
            code: JobAssetErrorCodes.InvalidName,
            message: $"The asset name '{name}' is invalid. Please check the name and try again.");
    }

    public static Error InvalidAssetId()
    {
        return Error.InvalidInput(
            code: JobAssetErrorCodes.InvalidAssetId,
            message: "The asset ID is invalid. Please check the ID and try again.");
    }
}
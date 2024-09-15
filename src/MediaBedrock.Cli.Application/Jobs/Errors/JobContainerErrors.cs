using Coderynx.Functional;
using Coderynx.Functional.Result;

namespace MediaBedrock.Cli.Application.Jobs.Errors;

public static class JobContainerErrors
{
    public static Error AssetNotFound(string name)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: "JobContainer.AssetNotFound",
            Message: $"Asset '{name}' not found in the job assets pool.");
    }

    public static Error AssetNotAvailable(string name)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: "JobContainer.AssetNotAvailable",
            Message: $"Asset {name} not available in the job assets pool.");
    }
}
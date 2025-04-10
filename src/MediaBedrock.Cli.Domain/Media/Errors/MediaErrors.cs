using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Media.Errors;

public static class MediaErrors
{
    public static Error FailedToRetrieveInfo(string uri)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: "Media.FailedToRetrieveInfo",
            Message: $"Failed to retrieve media information for {uri}");
    }
}
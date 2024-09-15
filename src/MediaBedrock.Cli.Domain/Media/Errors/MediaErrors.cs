using Coderynx.Functional;
using Coderynx.Functional.Result;

namespace MediaBedrock.Cli.Domain.Media.Errors;

public static class MediaErrors
{
    public static readonly Error FailedToRetrieveInfo = new(
        ResultError: ResultError.NotFound,
        Code: "Media.FailedToRetrieveInfo",
        Message: "Failed to retrieve media information.");
}
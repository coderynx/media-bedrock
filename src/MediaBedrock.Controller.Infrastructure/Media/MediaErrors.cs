using Coderynx.Functional.Results.Errors;

namespace MediaBedrock.Controller.Infrastructure.Media;

public static class MediaErrorCodes
{
    public const string ObjectNotFound = "Media.ObjectNotFound";
    public const string RetrievalFailed = "Media.RetrievalFailed";
}

public static class MediaErrors
{
    public static Error ObjectNotFound(string uri)
    {
        return Error.NotFound(
            code: MediaErrorCodes.ObjectNotFound,
            message: $"The media at '{uri}' was not found."
        );
    }
    
    public static Error RetrievalFailed(string uri)
    {
        return Error.InvalidInput(
            code: MediaErrorCodes.RetrievalFailed,
            message: $"The media at '{uri}' could not be retrieved."
        );
    }
}
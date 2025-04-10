using Coderynx.Functional.Results;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Cli.Application.Assets;

/// <summary>
///     Interface for retrieving media information.
/// </summary>
public interface IMediaInformationRetriever
{
    /// <summary>
    ///     Asynchronously retrieves media information for a given URI.
    /// </summary>
    /// <param name="uri">The URI of the media.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a <see cref="Result" /> object
    ///     with the media information.
    /// </returns>
    Task<Result<MediaInformation>> GetMediaInfoAsync(string uri);
}
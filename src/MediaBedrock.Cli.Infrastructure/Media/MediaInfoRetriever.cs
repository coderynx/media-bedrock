using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Application.Assets;
using MediaBedrock.Cli.Domain.Media.Errors;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Cli.Infrastructure.Media;

/// <summary>
///     Retrieves media information using the `mediainfo` command-line tool.
/// </summary>
public sealed class MediaInfoRetriever(ILogger<MediaInfoRetriever> logger) : IMediaInformationRetriever
{
    /// <inheritdoc />
    public async Task<Result<MediaInformation>> GetMediaInfoAsync(string uri)
    {
        logger.LogInformation("Retrieving media information for {Uri}", uri);

        var output = new StringBuilder();

        var exitCode = await SpawnProcessAsync(uri, (_, args) =>
        {
            if (args.Data is not null)
            {
                output.Append(args.Data);
            }
        });

        if (exitCode is not 0)
        {
            logger.LogError("Failed to retrieve media information for {Uri}", uri);
            return MediaErrors.FailedToRetrieveInfo(uri);
        }

        var jsonNode = JsonSerializer.Deserialize<JsonElement>(output.ToString());

        var general = jsonNode.GetProperty("media")
            .GetProperty("track")
            .EnumerateArray()
            .FirstOrDefault(t => t.GetProperty("@type").GetString() is "General");

        var format = general.GetProperty("Format").GetString() ?? "unknown";

        logger.LogInformation("Successfully retrieved media information for {Uri}", uri);

        var mediaInfo = new MediaInformation(format);
        return Result.Found(mediaInfo);
    }

    private static async Task<int> SpawnProcessAsync(
        string arguments = "",
        DataReceivedEventHandler? outputHandler = null)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "mediainfo",
                Arguments = $"--output=JSON {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        if (outputHandler is not null)
        {
            process.OutputDataReceived += outputHandler;
        }

        process.Start();
        process.BeginOutputReadLine();
        await process.WaitForExitAsync();

        return process.ExitCode;
    }
}
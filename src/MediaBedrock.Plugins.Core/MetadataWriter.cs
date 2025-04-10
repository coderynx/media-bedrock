using ATL;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Plugins.Core;

[Processor("core", "metadata-writer")]
public sealed class MetadataWriter(ILogger<MetadataWriter> logger) : IProcessor
{
    public async Task<ProcessorResult> ProcessAsync(ProcessorContext context, CancellationToken ct = default)
    {
        var inputTrack = context.GetInput("input");
        if (inputTrack is null)
        {
            logger.LogError("Input track not found");
            return ProcessorResult.Failure("Input track not found");
        }

        var outputTrack = context.GetOutput("output");
        if (outputTrack is null)
        {
            logger.LogError("Output track not found");
            return ProcessorResult.Failure("Output track not found");
        }

        await using var inputStream = inputTrack.GetAsStream();
        await using var outputStream = outputTrack.GetAsStream();

        var track = new Track(inputStream)
        {
            Title = context.GetProperty("Title")?.GetValue(string.Empty),
            Artist = context.GetProperty("Artist")?.GetValue(string.Empty),
            Album = context.GetProperty("Album")?.GetValue(string.Empty),
            Genre = context.GetProperty("Genre")?.GetValue(string.Empty),
            Comment = context.GetProperty("Comment")?.GetValue(string.Empty),
            Year = context.GetProperty("Year")?
                .Transform<int?>(input => int.TryParse(input, out var year) ? year : null)
        };

        if (int.TryParse(context.GetProperty("TrackNumber")?.GetValue(), out var trackNumber))
        {
            track.TrackNumber = trackNumber;
        }

        await track.SaveToAsync(outputStream);

        logger.LogInformation("Metadata written to {Input}", inputTrack);
        return ProcessorResult.Success();
    }
}
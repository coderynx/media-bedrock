using ATL;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Plugins.Core;

[Processor("core", "metadata_writer")]
public sealed class MetadataWriter : IProcessor
{
    public async Task<ProcessorResult> ProcessAsync(ProcessorContext context,
        CancellationToken cancellationToken = default)
    {
        const string inputKey = "input";
        const string outputKey = "output";
        const string titleKey = "title";
        const string artistKey = "artist";
        const string albumKey = "album";
        const string genreKey = "genre";
        const string commentKey = "comment";
        const string yearKey = "year";
        const string trackNumberKey = "track-number";

        var inputTrack = context.GetInput(inputKey);
        if (inputTrack is null)
        {
            return ProcessorResult.Failure($"{inputKey} track not found");
        }

        var outputTrack = context.GetOutput(outputKey);
        if (outputTrack is null)
        {
            return ProcessorResult.Failure($"{outputKey} track not found");
        }

        await using var inputStream = inputTrack.GetAsStream();
        await using var outputStream = outputTrack.GetAsStream();

        var track = new Track(inputStream)
        {
            Title = context.GetProperty(titleKey)?.GetValue(string.Empty),
            Artist = context.GetProperty(artistKey)?.GetValue(string.Empty),
            Album = context.GetProperty(albumKey)?.GetValue(string.Empty),
            Genre = context.GetProperty(genreKey)?.GetValue(string.Empty),
            Comment = context.GetProperty(commentKey)?.GetValue(string.Empty),
            Year = context.GetProperty(yearKey)?
                .Transform<int?>(input => int.TryParse(input, out var year) ? year : null)
        };

        if (int.TryParse(context.GetProperty(trackNumberKey)?.GetValue(), out var trackNumber))
        {
            track.TrackNumber = trackNumber;
        }

        await track.SaveToAsync(outputStream);

        return ProcessorResult.Success();
    }
}
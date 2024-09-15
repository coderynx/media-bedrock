using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Cli.Domain.Jobs;

public sealed class JobAsset(
    string name,
    string? uri,
    JobAssetKind kind,
    MediaInformation? mediaInformation = null)
{
    public string Name { get; init; } = name;
    public string? Uri { get; private set; } = uri;
    public JobAssetKind Kind { get; set; } = kind;
    public MediaInformation? MediaInformation { get; set; } = mediaInformation;

    public void UpdateUri(string? uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            return;
        }

        Uri = uri;
    }
}
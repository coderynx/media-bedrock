using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Cli.Domain.Jobs.Assets;

public sealed class JobAsset(
    string name,
    string? uri,
    JobAssetKind kind,
    MediaInformation? mediaInformation = null)
{
    public string Name { get; init; } = name;
    public bool IsAvailable => !string.IsNullOrWhiteSpace(Uri);
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

    public static JobAsset Create(string name, string uri)
    {
        return new JobAsset(name, uri, JobAssetKind.Input);
    }
}
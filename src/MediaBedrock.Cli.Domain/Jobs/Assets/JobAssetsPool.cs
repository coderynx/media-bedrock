using Coderynx.Functional.Options;

namespace MediaBedrock.Cli.Domain.Jobs.Assets;

public sealed class JobAssetsPool
{
    private readonly List<JobAsset> _assets = [];

    public void AddAsset(JobAsset asset)
    {
        if (_assets.Any(a => a.Name.Equals(asset.Name)))
        {
            return;
        }

        _assets.Add(asset);
    }

    public Option<JobAsset> ResolveAsset(string name)
    {
        var asset = _assets.FirstOrDefault(a => a.Name.Equals(name));

        return asset is null
            ? Option<JobAsset>.None()
            : Option<JobAsset>.Some(asset);
    }

    public Option<JobAsset> ResolveAsset(string name, JobAssetKind kind)
    {
        var asset = _assets.FirstOrDefault(a => a.Name.Equals(name) && a.Kind == kind);

        return asset is null
            ? Option<JobAsset>.None()
            : Option<JobAsset>.Some(asset);
    }

    public bool DoesAssetExist(string name)
    {
        return _assets.Any(a => a.Name.Equals(name));
    }
}
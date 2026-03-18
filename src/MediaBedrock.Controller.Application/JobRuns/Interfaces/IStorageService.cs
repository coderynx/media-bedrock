using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.JobAssets;

namespace MediaBedrock.Controller.Application.JobRuns.Interfaces;

public interface IStorageService
{
    Task<Result<JobAsset>> GetAssetAsync(JobAssetName assetName, CancellationToken cancellationToken = new());
    Task<Result<Uri>> CreateAssetUriAsync(JobAssetName assetName, CancellationToken cancellationToken = new());
}
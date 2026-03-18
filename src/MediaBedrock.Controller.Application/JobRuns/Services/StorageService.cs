using Coderynx.Functional.Results;
using MediaBedrock.Controller.Application.Database;
using MediaBedrock.Controller.Application.JobRuns.Interfaces;
using MediaBedrock.Controller.Domain.JobAssets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Controller.Application.JobRuns.Services;

public sealed class StorageService(IControllerDbContext controllerDbContext, ILogger<StorageService> logger) 
    : IStorageService
{
    public async Task<Result<JobAsset>> GetAssetAsync(JobAssetName assetName, CancellationToken cancellationToken = new())
    {
        var jobAsset = await controllerDbContext.JobAssets
            .AsNoTracking()
            .SingleOrDefaultAsync(j => j.Name.Equals(assetName), cancellationToken);

        if (jobAsset is null)
        {
            logger.LogDebug("Asset {JobAssetName} not found", assetName);
            return JobAssetErrors.NotFound(assetName);
        }
        
        logger.LogDebug("Found asset {JobAssetName}", assetName);
        
        return Result.Found(jobAsset);
    }

    public async Task<Result<Uri>> CreateAssetUriAsync(
        JobAssetName assetName,
        CancellationToken cancellationToken = new())
    {
        var jobAsset = await controllerDbContext.JobAssets
            .AsNoTracking()
            .SingleOrDefaultAsync(j => j.Name.Equals(assetName), cancellationToken);

        if (jobAsset is null)
        {
            return JobAssetErrors.NotFound(assetName);
        }

        if (!jobAsset.IsAvailable || jobAsset.Uri is null)
        {
            return JobAssetErrors.NotAvailable(assetName);
        }

        var uri = new Uri(jobAsset.Uri);

        logger.LogDebug("Created asset URI {Uri}", uri);
        return Result.Created(uri);
    }
}
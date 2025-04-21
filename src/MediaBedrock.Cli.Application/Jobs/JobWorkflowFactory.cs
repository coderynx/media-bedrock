using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Assets;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.Media;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Cli.Application.Jobs;

/// <inheritdoc />
public sealed class JobWorkflowFactory(
    IMediaInformationRetriever mediaInformationRetriever,
    ILogger<JobWorkflowFactory> logger) : IJobWorkflowFactory
{
    /// <inheritdoc />
    public async Task<Result<JobStateMachine>> CreateAsync(Job job)
    {
        JobAssetsPool assetsPool = new();

        foreach (var input in job.Inputs)
        {
            var getMediaInfo = await mediaInformationRetriever.GetMediaInfoAsync(input.Uri);
            if (getMediaInfo.IsFailure)
            {
                logger.LogError("Failed to get media information for asset {AssetName}", input.Name);
                return getMediaInfo.Error;
            }

            var asset = new JobAsset(input.Name, input.Uri, JobAssetKind.Input, getMediaInfo.Value);
            assetsPool.AddAsset(asset);
        }

        foreach (var output in job.Outputs)
        {
            var asset = new JobAsset(output.Name, output.FilePath, JobAssetKind.Output);
            assetsPool.AddAsset(asset);
        }

        var steps = new List<JobStep>();
        foreach (var step in job.Steps)
        {
            foreach (var input in step.Sinks)
            {
                if (assetsPool.DoesAssetExist(input.Name))
                {
                    continue;
                }

                var asset = new JobAsset(input.AssetName, null, JobAssetKind.Intermediate);
                assetsPool.AddAsset(asset);
            }

            foreach (var output in step.Sources)
            {
                if (assetsPool.DoesAssetExist(output.Name))
                {
                    continue;
                }

                var asset = new JobAsset(output.AssetName, null, JobAssetKind.Intermediate);
                assetsPool.AddAsset(asset);
            }

            steps.Add(step);
        }

        var container = new JobStateMachine(
            jobId: job.Id,
            activities: steps.Select(JobActivity.Create).ToList(),
            assetsPool: assetsPool);

        return Result.Created(container);
    }
}
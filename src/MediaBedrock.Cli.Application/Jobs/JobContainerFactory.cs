using Coderynx.Functional.Result;
using MediaBedrock.Cli.Application.Assets;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Cli.Application.Jobs;

/// <inheritdoc />
public sealed class JobContainerFactory(
    IMediaInformationRetriever mediaInformationRetriever,
    IProcessorProvider processorProvider,
    ILogger<JobContainerFactory> logger) : IJobContainerFactory
{
    /// <inheritdoc />
    public async Task<Result<JobContainer>> CreateAsync(Job job)
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

        var processors = new Dictionary<JobStep, IProcessor>();
        foreach (var step in job.Steps)
        {
            var resolveProcessor = processorProvider.ResolveProcessor(step.ProcessorName);
            if (resolveProcessor.IsFailure)
            {
                logger.LogError("Failed to resolve {ProcessorName} processor", step.ProcessorName);
                return resolveProcessor.Error;
            }

            processors.Add(step, resolveProcessor.Value);

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

            logger.LogInformation("Successfully resolved {ProcessorName} processor", step.ProcessorName);
        }

        var container = new JobContainer(
            job.Id,
            processors,
            assetsPool);
        
        return Result.Created(container);
    }
}
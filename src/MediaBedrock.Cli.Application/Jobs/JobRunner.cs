using Coderynx.Functional.Result;
using MediaBedrock.Cli.Application.Assets;
using MediaBedrock.Cli.Application.Jobs.Errors;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Processors;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Cli.Application.Jobs;

/// <inheritdoc />
public sealed class JobRunner(
    IJobContainerFactory jobContainerFactory,
    IProcessorContextFactory processorContextFactory,
    IMediaInformationRetriever mediaInformationRetriever,
    ILogger<JobRunner> logger) : IJobRunner
{
    /// <inheritdoc />
    public async Task<Result> TakeAsync(Job job, CancellationToken ct = default)
    {
        logger.LogInformation("Starting job execution for {JobId}", job.Id);

        var createContainer = await jobContainerFactory.CreateAsync(job);
        if (createContainer.IsFailure)
        {
            return createContainer.Error;
        }

        logger.LogInformation("Successfully initialized job {JobId} execution", job.Id);

        var container = createContainer.Value;
        foreach (var (step, processor) in container.Processors)
        {
            var createContext = processorContextFactory.Create(step, container.AssetsPool);
            if (createContext.IsFailure)
            {
                return createContext.Error;
            }

            var context = createContext.Value;

            var processResult = await processor.ProcessAsync(context, ct);
            if (!processResult.IsSuccess)
            {
                return ProcessorErrors.ProcessingFailed(job.Id, step.ProcessorName);
            }

            var updateAssetPool = await UpdateAssetPoolAsync(container, context);
            if (updateAssetPool.IsFailure)
            {
                return updateAssetPool.Error;
            }

            logger.LogInformation("Successfully processed step {StepName} of job {JobId}", step.Name, job.Id);
        }

        var tempFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
        Directory.Delete(tempFolder, true);

        logger.LogInformation("Successfully processed job {JobId}", job.Id);

        return Result.Accepted();
    }

    /// <inheritdoc />
    public async Task<Result> TakeAsync(List<Job> jobs, CancellationToken ct = default)
    {
        var tasks = jobs.Select(job => TakeAsync(job, ct));

        logger.LogInformation("Starting batch job execution for {JobCount} jobs", jobs.Count);

        var results = await Task.WhenAll(tasks);

        logger.LogInformation("Batch job execution completed for {JobCount} jobs", jobs.Count);

        return results.Any(result => !result.IsSuccess)
            ? results.First(result => !result.IsSuccess)
            : Result.Accepted();
    }

    private async Task<Result> UpdateAssetPoolAsync(JobContainer container, ProcessorContext context)
    {
        foreach (var output in context.Outputs)
        {
            var resolveAsset = container.AssetsPool.ResolveAsset(output.AssetName);
            if (!resolveAsset.IsSome)
            {
                return JobContainerErrors.AssetNotFound(output.AssetName);
            }

            var getMediaInfo = await mediaInformationRetriever.GetMediaInfoAsync(output.GetAsFilePath());
            if (getMediaInfo.IsFailure)
            {
                return getMediaInfo.Error;
            }

            var asset = resolveAsset.ValueOrThrow();
            asset.UpdateUri(output.GetAsFilePath());

            logger.LogInformation("Asset {AssetName} became available", output.AssetName);
        }

        return Result.Updated();
    }
}
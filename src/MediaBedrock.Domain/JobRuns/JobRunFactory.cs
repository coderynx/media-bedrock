using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobAssets.Interfaces;
using MediaBedrock.Domain.JobRuns.Interfaces;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Steps;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Domain.JobRuns;

/// <inheritdoc />
public sealed class JobRunFactory(
    IMediaInformationRetriever mediaInformationRetriever,
    ILogger<JobRunFactory> logger) : IJobRunFactory
{
    /// <inheritdoc />
    public async Task<Result<JobRun>> CreateAsync(Job job)
    {
        var jobRun = JobRun.Create(job.Id);

        var createInputAssets = await CreateInputAssets(job, jobRun);
        if (createInputAssets.IsFailure)
        {
            logger.LogError("Failed to create input assets for job {JobId}", job.Id);
            return createInputAssets.Error;
        }

        var createOutputAssets = CreateOutputAssets(job, jobRun);
        if (createOutputAssets.IsFailure)
        {
            logger.LogError("Failed to create output assets for job {JobId}", job.Id);
            return createOutputAssets.Error;
        }

        var createSteps = CreateSteps(job, jobRun);
        if (createSteps.IsFailure)
        {
            logger.LogError("Failed to create steps for job {JobId}", job.Id);
            return createSteps.Error;
        }

        return Result.Created(jobRun);
    }

    private async Task<Result> CreateInputAssets(Job job, JobRun jobRun)
    {
        foreach (var input in job.Inputs)
        {
            var getMediaInfo = await mediaInformationRetriever.GetMediaInfoAsync(input.Uri);
            if (getMediaInfo.IsFailure)
            {
                logger.LogError("Failed to get media information for asset {AssetName}", input.Name);
                return getMediaInfo.Error;
            }

            var createAsset = JobAsset.CreateInput(
                jobRun: jobRun,
                name: new JobAssetName(input.Name),
                uri: input.Uri,
                mediaInformation: getMediaInfo.Value);

            if (createAsset.IsFailure)
            {
                logger.LogError("Failed to create asset {AssetName}", input.Name);
                return createAsset.Error;
            }

            jobRun.AddAsset(createAsset.Value);
        }

        return Result.Created();
    }

    private Result CreateOutputAssets(Job job, JobRun jobRun)
    {
        foreach (var output in job.Outputs)
        {
            var createAsset = JobAsset.CreateOutput(jobRun, new JobAssetName(output.Name), output.FilePath);
            if (createAsset.IsFailure)
            {
                logger.LogError("Failed to create asset {AssetName}", output.Name);
                return createAsset.Error;
            }

            jobRun.AddAsset(createAsset.Value);
        }

        return Result.Created();
    }

    private static Result CreateSteps(Job job, JobRun jobRun)
    {
        foreach (var step in job.Steps)
        {
            var createStepInputAssets = PopulateStepInputAssets(step, jobRun);
            if (createStepInputAssets.IsFailure)
            {
                return createStepInputAssets.Error;
            }

            var createStepOutputAssets = PopulateStepOutputAssets(step, jobRun);
            if (createStepOutputAssets.IsFailure)
            {
                return createStepOutputAssets.Error;
            }

            jobRun.AddStep(step);
        }

        return Result.Created();
    }

    private static Result PopulateStepInputAssets(JobStep step, JobRun jobRun)
    {
        foreach (var inputAssetName in step.Inputs.Select(input => input.AssetName))
        {
            if (jobRun.DoesAssetExist(inputAssetName))
            {
                continue;
            }

            var createMezzanine = JobAsset.CreateMezzanine(jobRun, inputAssetName);
            if (createMezzanine.IsFailure)
            {
                return createMezzanine.Error;
            }

            jobRun.AddAsset(createMezzanine.Value);
        }

        return Result.Created();
    }

    private static Result PopulateStepOutputAssets(JobStep step, JobRun jobRun)
    {
        foreach (var outputAssetName in step.Outputs.Select(output => output.AssetName))
        {
            if (jobRun.DoesAssetExist(outputAssetName))
            {
                continue;
            }

            var createMezzanine = JobAsset.CreateMezzanine(jobRun, outputAssetName);
            if (createMezzanine.IsFailure)
            {
                return createMezzanine.Error;
            }

            jobRun.AddAsset(createMezzanine.Value);
        }

        return Result.Created();
    }
}
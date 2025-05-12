using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobAssets.Interfaces;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.JobStateMachine.Interfaces;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Domain.JobStateMachine;

/// <inheritdoc />
public sealed class JobStateMachineFactory(
    IMediaInformationRetriever mediaInformationRetriever,
    ILogger<JobStateMachineFactory> logger) : IJobStateMachineFactory
{
    /// <inheritdoc />
    public async Task<Result<JobStateMachine>> CreateAsync(Job job)
    {
        var jobStateMachine = JobStateMachine.Create(job);

        var createInputAssets = await CreateInputAssets(job, jobStateMachine);
        if (createInputAssets.IsFailure)
        {
            logger.LogError("Failed to create input assets for job {JobId}", job.Id);
            return createInputAssets.Error;
        }

        var createOutputAssets = CreateOutputAssets(job, jobStateMachine);
        if (createOutputAssets.IsFailure)
        {
            logger.LogError("Failed to create output assets for job {JobId}", job.Id);
            return createOutputAssets.Error;
        }

        var createSteps = CreateSteps(job, jobStateMachine);
        if (createSteps.IsFailure)
        {
            logger.LogError("Failed to create steps for job {JobId}", job.Id);
            return createSteps.Error;
        }

        return Result.Created(jobStateMachine);
    }

    private async Task<Result> CreateInputAssets(Job job, JobStateMachine jobStateMachine)
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
                jobStateMachine: jobStateMachine,
                name: new JobAssetName(input.Name),
                uri: input.Uri,
                mediaInformation: getMediaInfo.Value);

            if (createAsset.IsFailure)
            {
                logger.LogError("Failed to create asset {AssetName}", input.Name);
                return createAsset.Error;
            }

            jobStateMachine.AddAsset(createAsset.Value);
        }

        return Result.Created();
    }

    private Result CreateOutputAssets(Job job, JobStateMachine jobStateMachine)
    {
        foreach (var output in job.Outputs)
        {
            var createAsset = JobAsset.CreateOutput(jobStateMachine, new JobAssetName(output.Name), output.FilePath);
            if (createAsset.IsFailure)
            {
                logger.LogError("Failed to create asset {AssetName}", output.Name);
                return createAsset.Error;
            }

            jobStateMachine.AddAsset(createAsset.Value);
        }

        return Result.Created();
    }

    private static Result CreateSteps(Job job, JobStateMachine jobStateMachine)
    {
        foreach (var step in job.Steps)
        {
            foreach (var input in step.Inputs)
            {
                if (jobStateMachine.DoesAssetExist(input.AssetName))
                {
                    continue;
                }

                var createMezzanine = JobAsset.CreateMezzanine(jobStateMachine, input.AssetName);
                if (createMezzanine.IsFailure)
                {
                    return createMezzanine.Error;
                }

                jobStateMachine.AddAsset(createMezzanine.Value);
            }

            foreach (var output in step.Outputs)
            {
                if (jobStateMachine.DoesAssetExist(output.AssetName))
                {
                    continue;
                }

                var createMezzanine = JobAsset.CreateMezzanine(jobStateMachine, output.AssetName);
                if (createMezzanine.IsFailure)
                {
                    return createMezzanine.Error;
                }

                jobStateMachine.AddAsset(createMezzanine.Value);
            }

            jobStateMachine.AddStep(step);
        }

        return Result.Created();
    }
}
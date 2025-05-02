using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.JobAssets.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Cli.Application.Jobs;

/// <inheritdoc />
public sealed class JobStateMachineFactory(
    IMediaInformationRetriever mediaInformationRetriever,
    ILogger<JobStateMachineFactory> logger) : IJobStateMachineFactory
{
    /// <inheritdoc />
    public async Task<Result<JobStateMachine>> CreateAsync(Job job)
    {
        var jobStateMachine = JobStateMachine.Create(job);

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

        var steps = new List<JobStep>();
        foreach (var step in job.Steps)
        {
            foreach (var input in step.Sinks)
            {
                if (jobStateMachine.DoesAssetExist(input.AssetName))
                {
                    continue;
                }

                var createAsset = JobAsset.CreateMezzanine(jobStateMachine, input.AssetName);
                if (createAsset.IsFailure)
                {
                    return createAsset.Error;
                }

                jobStateMachine.AddAsset(createAsset.Value);
            }

            foreach (var output in step.Sources)
            {
                if (jobStateMachine.DoesAssetExist(output.AssetName))
                {
                    continue;
                }

                var createAsset = JobAsset.CreateMezzanine(jobStateMachine, output.AssetName);
                if (createAsset.IsFailure)
                {
                    return createAsset.Error;
                }

                jobStateMachine.AddAsset(createAsset.Value);
            }

            steps.Add(step);
        }

        var jobStepsStateMachines = steps.Select(JobStepStateMachine.Create);
        jobStateMachine.AddStepRange(jobStepsStateMachines);

        return Result.Created(jobStateMachine);
    }
}
using Coderynx.Functional.Results;
using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Controller.Application.Database;
using MediaBedrock.Controller.Application.JobRuns.Interfaces;
using MediaBedrock.Controller.Domain.JobAssets;
using MediaBedrock.Controller.Domain.JobRuns;
using MediaBedrock.Controller.Domain.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Controller.Application.JobRuns.Services;

public sealed class JobRunStepsOrchestrator(
    IControllerDbContext dbContext,
    IMessagePublisher messagPublisher,
    IMediaInformationRetriever mediaInformationRetriever,
    ILogger<JobRunStepsOrchestrator> logger) : IJobRunStepsOrchestrator
{
    public async Task<Result> CompleteAsync(JobRunStepId jobRunStepId, CancellationToken cancellationToken = new())
    {
        var jobRunStep = await dbContext.JobRunSteps.SingleOrDefaultAsync(
            jssm => jssm.Id.Equals(jobRunStepId),
            cancellationToken);

        if (jobRunStep is null)
        {
            return JobRunErrors.NotFound(jobRunStepId);
        }
        
        var updateAssetPool = await UpdateAssetPoolAsync(
            jobRunStep.StepOutputs.Select(so => so.AssetName).ToList(),
            cancellationToken);
        
        if (updateAssetPool.IsFailure)
        {
            var jobStepError = new JobRunStepError(
                reason: JobStepFailureReason.OutputAssetsAssessment,
                message: updateAssetPool.Error.Message);

            var transitionToFailed = jobRunStep.Fail(jobStepError);
            if (transitionToFailed.IsFailure)
            {
                logger.LogError(
                    "Failed to transition step {StepName} of job execution {JobRunId} to failed: {ErrorMessage}",
                    jobRunStep.StepName,
                    jobRunStep.JobRun.Id,
                    transitionToFailed.Error.Message);

                return transitionToFailed.Error;
            }

            logger.LogError("Failed to update asset pool for job {JobRunStepId}: {ErrorMessage}",
                jobRunStepId,
                updateAssetPool.Error.Message);
            
            return updateAssetPool.Error;
        }

        var transitionToCompleted = jobRunStep.Complete();
        if (transitionToCompleted.IsFailure)
        {
            return transitionToCompleted.Error;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        
        logger.LogInformation("Successfully processed step {StepName} of job execution {JobRunId}",
            jobRunStep.StepName,
            jobRunStep.JobRun.Id);
        
        return Result.Updated();
    }

    public async Task<Result> FailAsync(
        JobRunStepId jobRunStepId,
        JobStepFailureReason reason,
        string message = "",
        CancellationToken cancellationToken = new())
    {
        var jobRunStep = await dbContext.JobRunSteps
            .Include(jssm => jssm.JobRun)
            .SingleOrDefaultAsync(j => j.Id.Equals(jobRunStepId), cancellationToken);

        if (jobRunStep is null)
        {
            return JobRunErrors.NotFound(jobRunStepId);
        }

        var jobStepError = new JobRunStepError(reason, message);

        var transitionToFailed = jobRunStep.Fail(jobStepError);
        if (transitionToFailed.IsFailure)
        {
            return transitionToFailed.Error;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        // TODO: Implement failure reason creation.
        var jobFailed = new JobRunFailedIntegrationEvent(
            jobRunStep.JobRun.Id.Value,
            nameof(JobFailureReason.Processing),
            message);

        await messagPublisher.PublishAsync(jobFailed, cancellationToken);

        return Result.Updated();
    }

    private async Task<Result> UpdateAssetPoolAsync(
        List<JobAssetName> assetNames, 
        CancellationToken cancellationToken = new())
    {
        foreach (var assetName in assetNames)
        {
            var asset = await dbContext.JobAssets.SingleOrDefaultAsync(
                ja => ja.Name.Equals(assetName),
                cancellationToken);

            if (asset is null)
            {
                return JobAssetErrors.NotFound(assetName);
            }

            if (asset.Uri is null)
            {
                return JobAssetErrors.NotAvailable(assetName);
            }

            var getMediaInfo = await mediaInformationRetriever.RetrieveAsync(asset.Uri);
            if (getMediaInfo.IsFailure)
            {
                return getMediaInfo.Error;
            }

            var mediaInformation = getMediaInfo.Value;
            asset.MakeAvailable(mediaInformation);

            logger.LogInformation("Asset {AssetName} became available", assetName);
        }

        return Result.Updated();
    }
}
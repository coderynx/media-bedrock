using Coderynx.Functional.Results;
using MediaBedrock.Cli.Application.Jobs.Handlers;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Batches;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Cli.Application.Jobs;

/// <inheritdoc />
public sealed class JobRunner(
    IJobWorkflowFactory jobWorkflowFactory,
    IJobMessageBus messageBus,
    IJobWorkflowRepository jobWorkflowRepository,
    ILogger<JobRunner> logger) : IJobRunner
{
    /// <inheritdoc />
    public async Task<Result> TakeAsync(Job job, CancellationToken ct = default)
    {
        logger.LogInformation("Starting job execution for {JobId}", job.Id);

        var createContainer = await jobWorkflowFactory.CreateAsync(job);
        if (createContainer.IsFailure)
        {
            return createContainer.Error;
        }

        logger.LogInformation("Successfully initialized job {JobId} execution", job.Id);

        var workflow = createContainer.Value;

        jobWorkflowRepository.Store(workflow);

        var startJob = RunJob.Create(workflow);
        await messageBus.PublishAsync(startJob, ct);

        while (workflow.Status is not JobWorkflowStatus.Completed)
        {
            if (ct.IsCancellationRequested)
            {
                logger.LogWarning("Job execution for {JobId} was canceled", job.Id);
                return Result.Accepted();
            }

            if (workflow.Status is JobWorkflowStatus.Failed)
            {
                logger.LogError("Job execution for {JobId} failed", job.Id);
                return Result.Accepted();
            }

            await Task.Delay(500, ct);
        }

        logger.LogInformation("Successfully processed job {JobId}", job.Id);

        return Result.Accepted();
    }

    /// <inheritdoc />
    public async Task<Result> TakeAsync(BatchJob batchJob, CancellationToken ct = default)
    {
        var tasks = batchJob.Jobs.Select(job => TakeAsync(job, ct));

        logger.LogInformation("Starting batch job execution for {JobCount} jobs", batchJob.Jobs.Count());

        var results = await Task.WhenAll(tasks);

        var errors = results.Where(result => !result.IsSuccess).ToList();
        if (errors.Count is not 0)
        {
            logger.LogError("Batch job execution failed for {JobCount} jobs", errors.Count);
            return Result.Failure(errors.First().Error);
        }

        logger.LogInformation("Batch job execution completed for {JobCount} jobs", batchJob.Jobs.Count());

        return Result.Accepted();
    }
}
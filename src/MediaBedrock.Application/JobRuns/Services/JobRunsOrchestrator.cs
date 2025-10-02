using Coderynx.Functional.Results;
using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Application.Database;
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Application.JobRuns.Services;

public sealed class JobRunsOrchestrator(
    IApplicationDbContext dbContext,
    IMessagePublisher messagePublisher,
    ILogger<JobRunsOrchestrator> logger) : IJobRunsOrchestrator
{
    public async Task<Result> StartAsync(
        JobRunId jobRunId,
        CancellationToken cancellationToken = new())
    {
        var jobRun = await dbContext.JobRuns
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .Include(j => j.Steps)
            .ThenInclude(s => s.StepInputs)
            .Include(j => j.Steps)
            .ThenInclude(s => s.StepOutputs)
            .AsNoTracking()
            .SingleOrDefaultAsync(j => j.Id.Equals(jobRunId), cancellationToken);

        if (jobRun is null)
        {
            return JobRunErrors.NotFound(jobRunId);
        }

        var inputAssets = jobRun.ResolveAssets(JobAssetKind.Input);

        var processJobSteps = jobRun.Steps
            .Where(s => s.StepInputs.Any(a => inputAssets.Any(i => i.Name.Equals(a.AssetName))))
            .Select(jobStep => new ProcessJobRunStep(jobRun.Id.Value, jobStep.Id.Value));

        await messagePublisher.PublishAsync(processJobSteps, cancellationToken);

        jobRun.TransitionToRunning();
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Started job execution {JobRunId} for job {JobId}",
            jobRun.Id,
            jobRun.Job.Id);

        return Result.Accepted();
    }

    public async Task<Result> CompleteAsync(
        JobRunId jobRunId,
        CancellationToken cancellationToken = new())
    {
        var jobRun = await dbContext.JobRuns
            .SingleOrDefaultAsync(j => j.Id.Equals(jobRunId), cancellationToken: cancellationToken);

        if (jobRun is null)
        {
            return JobRunErrors.NotFound(jobRunId);
        }

        jobRun.TransitionToCompleted();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Updated();
    }

    public async Task<Result> FailAsync(
        JobRunId jobRunId,
        JobFailureReason failureReason,
        string message,
        CancellationToken cancellationToken = new())
    {
        var jobRun = await dbContext.JobRuns
            .SingleOrDefaultAsync(j => j.Id.Equals(jobRunId), cancellationToken);

        if (jobRun is null)
        {
            return JobRunErrors.NotFound(jobRunId);
        }

        jobRun.TransitionToFailed(failureReason, message);

        return await dbContext.SaveChangesAsync(cancellationToken) > 0
            ? Result.Updated()
            : JobRunErrors.UpdateFailed(jobRunId);
    }

    public async Task<Result> WaitForCompletionAsync(
        JobRunId runId,
        TimeSpan delayTime,
        CancellationToken cancellationToken = new())
    {
        var jobRun = await dbContext.JobRuns
            .AsNoTracking()
            .SingleOrDefaultAsync(jsm => jsm.Id.Equals(runId), cancellationToken);

        if (jobRun is null)
        {
            return JobErrors.RunNotFound(runId);
        }

        while (jobRun.Status is not JobRunStatus.Completed)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("Job execution for {JobId} was canceled", runId);
                return Result.Accepted();
            }

            if (jobRun.Status is JobRunStatus.Failed)
            {
                logger.LogError("Job execution for {JobId} failed", runId);
                return Result.Accepted();
            }

            await Task.Delay(delayTime, cancellationToken);

            jobRun = await dbContext.JobRuns
                .AsNoTracking()
                .FirstAsync(jsm => jsm.Id.Equals(runId), cancellationToken);
        }

        return Result.Accepted();
    }
}
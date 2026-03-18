using Coderynx.Functional.Results;
using MediaBedrock.Controller.Application.Database;
using MediaBedrock.Controller.Application.JobRuns.Interfaces;
using MediaBedrock.Controller.Domain.JobRuns;
using MediaBedrock.Controller.Domain.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Controller.Application.JobRuns.Services;

public sealed class JobRunsOrchestrator(IControllerDbContext dbContext, ILogger<JobRunsOrchestrator> logger)
    : IJobRunsOrchestrator
{
    public async Task<Result> StartAsync(JobRunId jobRunId, CancellationToken cancellationToken = new())
    {
        var jobRun = await dbContext.JobRuns.SingleOrDefaultAsync(j => j.Id.Equals(jobRunId), cancellationToken);
        if (jobRun is null)
        {
            return JobRunErrors.NotFound(jobRunId);
        }

        var startJobRun = jobRun.Start();
        if (startJobRun.IsFailure)
        {
            return startJobRun.Error;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Started job run {JobRunId} for job {JobId}",
            jobRun.Id,
            jobRun.JobId);

        return Result.Accepted();
    }

    public async Task<Result> FailAsync(
        JobRunId jobRunId,
        JobFailureReason failureReason,
        string message,
        CancellationToken cancellationToken = new())
    {
        var jobRun = await dbContext.JobRuns.SingleOrDefaultAsync(j => j.Id.Equals(jobRunId), cancellationToken);
        if (jobRun is null)
        {
            return JobRunErrors.NotFound(jobRunId);
        }

        var fail = jobRun.Fail(failureReason, message);
        if (fail.IsFailure)
        {
            return fail.Error;
        }

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
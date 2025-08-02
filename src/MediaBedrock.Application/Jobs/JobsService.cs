using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Application.Persistence;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.JobRuns.Interfaces;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Interfaces;
using MediaBedrock.Domain.Jobs.Parameters;
using MediaBedrock.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Application.Jobs;

public sealed class JobsService(
    ILogger<JobsService> logger,
    IJobFactory jobFactory,
    IJobRunFactory jobRunFactory,
    IMessageBus messageBus,
    IApplicationDbContext dbContext) : IJobsService
{
    public async Task<List<Job>> GetAsync(CancellationToken cancellationToken = default)
    {
        var jobs = await dbContext.Jobs
            .Include(j => j.Template)
            .Include(j => j.Steps)
            .ToListAsync(cancellationToken);

        logger.LogDebug("Retrieved {JobCount} jobs", jobs.Count);
        return jobs;
    }

    public async Task<Option<Job>> GetAsync(JobId id, CancellationToken cancellationToken = default)
    {
        var job = await dbContext.Jobs
            .Include(j => j.Template)
            .Include(j => j.Steps)
            .SingleOrDefaultAsync(j => j.Id.Equals(id), cancellationToken);

        if (job is null)
        {
            return Option.None<Job>();
        }

        logger.LogDebug("Retrieved job with ID {JobId} ", id);
        return Option.Some(job);
    }

    public async Task<Result<Job>> CreateAsync(
        JobTemplate jobTemplate,
        JobParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var createJob = jobFactory.Create(jobTemplate, parameters);
        if (createJob.IsFailure)
        {
            return createJob.Error;
        }

        await dbContext.Jobs.AddAsync(createJob.Value, cancellationToken);

        return await dbContext.SaveChangesAsync(cancellationToken) > 0
            ? Result.Created(createJob.Value)
            : JobErrors.StoreFailed(createJob.Value.Id);
    }

    public async Task<Result<JobRunId>> StartAsync(JobId jobId, CancellationToken cancellationToken = default)
    {
        var job = await dbContext.Jobs
            .Include(j => j.Template)
            .Include(j => j.Steps)
            .SingleOrDefaultAsync(j => j.Id.Equals(jobId), cancellationToken);

        if (job is null)
        {
            return JobErrors.NotFound(jobId);
        }

        logger.LogInformation("Starting job execution for {JobId}", job.Id);

        var createJobRun = await jobRunFactory.CreateAsync(job);
        if (createJobRun.IsFailure)
        {
            return createJobRun.Error;
        }

        var jobRun = createJobRun.Value;

        logger.LogInformation("Successfully initialized job {JobId} with run id {JobRunId}",
            job.Id,
            jobRun.Id);

        await dbContext.JobRuns.AddAsync(jobRun, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var startJob = new RunJob(jobRun.Id);
        await messageBus.PublishAsync(startJob, cancellationToken);

        logger.LogInformation("Successfully processed job {JobId}", job.Id);

        return Result.Created(jobRun.Id);
    }

    /// <inheritdoc />
    public async Task<Result<List<JobRunId>>> StartAsync(
        List<JobId> jobIds,
        CancellationToken cancellationToken = default)
    {
        var tasks = jobIds.Select(id => StartAsync(id, cancellationToken));

        logger.LogInformation("Starting batch job execution for {JobCount} jobs", jobIds.Count);

        var results = await Task.WhenAll(tasks);

        var errors = results.Where(result => !result.IsSuccess).ToList();
        if (errors.Count is not 0)
        {
            logger.LogError("Batch job execution failed for {JobCount} jobs", errors.Count);
            return errors.First().Error;
        }

        logger.LogInformation("Batch job execution completed for {JobCount} jobs", jobIds.Count);

        var jobRuns = results.Select(r => r.Value).ToList();
        return Result.Created(jobRuns);
    }

    public async Task<Result> WaitForCompletionAsync(
        JobRunId runId,
        TimeSpan delayTime,
        CancellationToken cancellationToken = default)
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
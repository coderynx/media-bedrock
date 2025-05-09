using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Application.Persistence;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Interfaces;
using MediaBedrock.Domain.Jobs.Parameters;
using MediaBedrock.Domain.JobStateMachine;
using MediaBedrock.Domain.JobStateMachine.Interfaces;
using MediaBedrock.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Application.Jobs;

public sealed class JobsService(
    ILogger<JobsService> logger,
    IJobFactory jobFactory,
    IJobStateMachineFactory jobStateMachineFactory,
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

    public async Task<Result<JobStateMachineId>> StartAsync(JobId jobId, CancellationToken cancellationToken = default)
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

        var createStateMachine = await jobStateMachineFactory.CreateAsync(job);
        if (createStateMachine.IsFailure)
        {
            return createStateMachine.Error;
        }

        var stateMachine = createStateMachine.Value;

        logger.LogInformation("Successfully initialized job {JobId} with execution {StateMachineId}",
            job.Id,
            stateMachine.Id);

        await dbContext.JobStateMachines.AddAsync(stateMachine, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var startJob = new RunJob(stateMachine.Id);
        await messageBus.PublishAsync(startJob, cancellationToken);

        logger.LogInformation("Successfully processed job {JobId}", job.Id);

        return Result.Created(stateMachine.Id);
    }

    /// <inheritdoc />
    public async Task<Result<List<JobStateMachineId>>> StartAsync(
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

        var stateMachines = results.Select(r => r.Value).ToList();
        return Result.Created(stateMachines);
    }

    public async Task<Result> WaitForCompletionAsync(
        JobStateMachineId stateMachineId,
        TimeSpan delayTime,
        CancellationToken cancellationToken = default)
    {
        var stateMachine = await dbContext.JobStateMachines
            .AsNoTracking()
            .SingleOrDefaultAsync(jsm => jsm.Id.Equals(stateMachineId), cancellationToken);

        if (stateMachine is null)
        {
            return JobErrors.StateMachineNotFound(stateMachineId);
        }

        while (stateMachine.ExecutionStatus is not JobExecutionStatus.Completed)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("Job execution for {JobId} was canceled", stateMachineId);
                return Result.Accepted();
            }

            if (stateMachine.ExecutionStatus is JobExecutionStatus.Failed)
            {
                logger.LogError("Job execution for {JobId} failed", stateMachineId);
                return Result.Accepted();
            }

            await Task.Delay(delayTime, cancellationToken);

            stateMachine = await dbContext.JobStateMachines
                .AsNoTracking()
                .FirstAsync(jsm => jsm.Id.Equals(stateMachineId), cancellationToken);
        }

        return Result.Accepted();
    }
}
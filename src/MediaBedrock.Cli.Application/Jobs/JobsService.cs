using Coderynx.Functional.Results;
using MediaBedrock.Cli.Application.Jobs.Handlers;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Application.Persistence;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Cli.Application.Jobs;

public sealed class JobsService(
    ILogger<JobsService> logger,
    IJobFactory jobFactory,
    IJobStateMachineFactory jobStateMachineFactory,
    IJobMessageBus messageBus,
    IApplicationDbContext dbContext) : IJobsService
{
    public async Task<Result<Job>> CreateAsync(JobTemplate jobTemplate, JobParameters parameters)
    {
        var createJob = jobFactory.Create(jobTemplate, parameters);
        if (createJob.IsFailure)
        {
            return createJob.Error;
        }

        await dbContext.Jobs.AddAsync(createJob.Value);
        await dbContext.SaveChangesAsync();

        return Result.Created(createJob.Value);
    }

    public async Task<Result<JobStateMachineId>> StartAsync(JobId jobId, CancellationToken ct = default)
    {
        var job = await dbContext.Jobs
            .Include(j => j.Template)
            .Include(j => j.Steps)
            .FirstOrDefaultAsync(j => j.Id.Equals(jobId), ct);

        if (job is null)
        {
            return JobErrors.NotFound(jobId);
        }

        logger.LogInformation("Starting job execution for {JobId}", job.Id);

        var createContainer = await jobStateMachineFactory.CreateAsync(job);
        if (createContainer.IsFailure)
        {
            return createContainer.Error;
        }

        logger.LogInformation("Successfully initialized job {JobId} execution", job.Id);

        var stateMachine = createContainer.Value;

        await dbContext.JobsStateMachines.AddAsync(stateMachine, ct);
        await dbContext.SaveChangesAsync(ct);

        var startJob = RunJob.Create(stateMachine);
        await messageBus.PublishAsync(startJob, ct);

        logger.LogInformation("Successfully processed job {JobId}", job.Id);

        return Result.Created(stateMachine.Id);
    }

    /// <inheritdoc />
    public async Task<Result<List<JobStateMachineId>>> RunAsync(List<JobId> jobIds, CancellationToken ct = default)
    {
        var tasks = jobIds.Select(id => StartAsync(id, ct));

        logger.LogInformation("Starting batch job execution for {JobCount} jobs", jobIds.Count);

        var results = await Task.WhenAll(tasks);

        var errors = results.Where(result => !result.IsSuccess).ToList();
        if (errors.Count is not 0)
        {
            logger.LogError("Batch job execution failed for {JobCount} jobs", errors.Count);
            return Result.Failure<List<JobStateMachineId>>(errors.First().Error);
        }

        logger.LogInformation("Batch job execution completed for {JobCount} jobs", jobIds.Count);

        var stateMachines = results.Select(r => r.Value).ToList();
        return Result.Created(stateMachines);
    }

    public async Task<Result> WaitForCompletionAsync(
        JobStateMachineId stateMachineId,
        TimeSpan delayTime,
        CancellationToken ct = default)
    {
        var stateMachine = await dbContext.JobsStateMachines
            .AsNoTracking()
            .FirstOrDefaultAsync(jsm => jsm.Id.Equals(stateMachineId), ct);

        if (stateMachine is null)
        {
            return JobErrors.StateMachineNotFound(stateMachineId);
        }

        while (stateMachine.Status is not JobStatus.Completed)
        {
            stateMachine = await dbContext.JobsStateMachines
                .AsNoTracking()
                .FirstAsync(jsm => jsm.Id.Equals(stateMachineId), ct);

            if (ct.IsCancellationRequested)
            {
                logger.LogWarning("Job execution for {JobId} was canceled", stateMachineId);
                return Result.Accepted();
            }

            if (stateMachine.Status is JobStatus.Failed)
            {
                logger.LogError("Job execution for {JobId} failed", stateMachineId);
                return Result.Accepted();
            }

            await Task.Delay(delayTime, ct);
        }

        return Result.Accepted();
    }
}
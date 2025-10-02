using Coderynx.Functional.Results;
using MediaBedrock.Application.Database;
using MediaBedrock.Application.JobRuns.Messages;
using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.JobRuns.Interfaces;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Interfaces;
using MediaBedrock.Domain.Jobs.Parameters;
using MediaBedrock.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Application.Jobs.Services;

public sealed class JobsService(
    ILogger<JobsService> logger,
    IJobFactory jobFactory,
    IJobRunFactory jobRunFactory,
    IMessageBus messageBus,
    IApplicationDbContext dbContext) : IJobsService
{
    public async Task<Result<Job>> CreateAsync(
        JobTemplate jobTemplate,
        JobParameters parameters,
        CancellationToken cancellationToken = new())
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

    public async Task<Result<JobRunId>> StartAsync(JobId jobId, CancellationToken cancellationToken = new())
    {
        var job = await dbContext.Jobs
            .AsNoTracking()
            .Include(j => j.Template)
            .Include(j => j.Steps)
            .SingleOrDefaultAsync(j => j.Id.Equals(jobId), cancellationToken);

        if (job is null)
        {
            return JobErrors.NotFound(jobId);
        }

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

        var startJob = new StartJobRun(jobRun.Id);
        await messageBus.PublishAsync(startJob, cancellationToken);

        logger.LogInformation("Successfully processed job {JobId}", job.Id);

        return Result.Created(jobRun.Id);
    }
}
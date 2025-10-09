using Coderynx.Functional.Results;
using MediaBedrock.Application.Database;
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.JobRuns.Interfaces;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Application.JobRuns.Services;

public sealed class JobRunService(
    IApplicationDbContext dbContext,
    IJobRunFactory jobRunFactory,
    ILogger<JobRunService> logger) : IJobRunService
{
    public async Task<Result<JobRunId>> CreateAsync(JobId jobId, CancellationToken cancellationToken = new())
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

        await dbContext.JobRuns.AddAsync(jobRun, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully initialized JobRun {JobRunId} from Job {JobId}",
            jobRun.Id,
            job.Id);

        return Result.Created(jobRun.Id);
    }

    public async Task<List<JobRun>> GetAsync(JobTemplateName jobTemplateName, CancellationToken ct = default)
    {
        var jobRuns = await dbContext.JobRuns
            .Include(jsm => jsm.AssetsPool)
            .Include(jsm => jsm.Job)
            .ThenInclude(j => j.Template)
            .Include(jsm => jsm.Steps)
            .ThenInclude(jssm => jssm.StepInputs)
            .Include(jsm => jsm.Steps)
            .ThenInclude(jssm => jssm.StepOutputs)
            .Where(jsm => jsm.Job.Template.Name.Equals(jobTemplateName))
            .AsNoTracking()
            .ToListAsync(cancellationToken: ct);

        logger.LogDebug("Retrieved job run with JobTemplateName {JobTemplateName} ", jobTemplateName);
        return jobRuns;
    }

    public async Task DeleteAsync(
        JobTemplateName jobTemplateName,
        JobRunStatus jobRunStatus = JobRunStatus.Pending,
        CancellationToken cancellationToken = default)
    {
        var jobRunCount = await dbContext.JobRuns
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .ThenInclude(j => j.Template)
            .Where(j => j.Job.Template.Name.Equals(jobTemplateName) && j.Status.Equals(jobRunStatus))
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);

        logger.LogInformation("Deleted {JobRunCount} job runs for template {JobTemplateName}",
            jobRunCount,
            jobTemplateName);
    }
}
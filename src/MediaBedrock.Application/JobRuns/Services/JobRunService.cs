using Coderynx.Functional.Options;
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.Persistence;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Application.JobRuns.Services;

public sealed class JobRunService(IApplicationDbContext dbContext, ILogger<JobRunService> logger) : IJobRunService
{
    public async Task<Option<JobRun>> GetAsync(JobId jobId, CancellationToken ct = default)
    {
        var jobRuns = await dbContext.JobRuns
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .Include(j => j.Steps)
            .ThenInclude(s => s.StepInputs)
            .Include(j => j.Steps)
            .ThenInclude(s => s.StepOutputs)
            .SingleOrDefaultAsync(j => j.Job.Id.Equals(jobId), cancellationToken: ct);

        if (jobRuns is null)
        {
            return Option.None<JobRun>();
        }

        logger.LogDebug("Retrieved job run with JobId {JobId} ", jobId);
        return Option.Some(jobRuns);
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
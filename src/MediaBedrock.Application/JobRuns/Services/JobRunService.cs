using Coderynx.Functional.Results;
using MediaBedrock.Application.Database;
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.JobRuns.Interfaces;
using MediaBedrock.Domain.Jobs;
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
            .Include(j => j.Steps.OrderBy(s => s.Order))
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
}
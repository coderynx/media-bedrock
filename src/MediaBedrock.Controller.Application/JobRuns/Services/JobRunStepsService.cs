using Coderynx.Functional.Results;
using MediaBedrock.Controller.Application.Database;
using MediaBedrock.Controller.Application.JobRuns.Interfaces;
using MediaBedrock.Controller.Domain.JobRuns;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Controller.Application.JobRuns.Services;

public sealed class JobRunStepsService(IControllerDbContext controllerDbContext, ILogger<JobRunStepsService> logger) 
    : IJobRunStepsService
{
    public async Task<Result<JobRunStep>> GetAsync(JobRunStepId id, CancellationToken cancellationToken = new())
    {
        var jobRunStep = await controllerDbContext.JobRunSteps
            .AsNoTracking()
            .SingleOrDefaultAsync(j => j.Id.Equals(id), cancellationToken);

        if (jobRunStep is null)
        {
            return JobRunErrors.NotFound(id);
        }

        logger.LogDebug("Job run step {JobRunStepId} found", id);

        return Result.Found(jobRunStep);
    }
}
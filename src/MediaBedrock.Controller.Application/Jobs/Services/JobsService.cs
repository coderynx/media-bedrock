using Coderynx.Functional.Results;
using MediaBedrock.Controller.Application.Database;
using MediaBedrock.Controller.Application.Jobs.Interfaces;
using MediaBedrock.Controller.Domain.Jobs;
using MediaBedrock.Controller.Domain.Jobs.Interfaces;
using MediaBedrock.Controller.Domain.Jobs.Parameters;
using MediaBedrock.Controller.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Controller.Application.Jobs.Services;

public sealed class JobsService(IJobFactory jobFactory, IControllerDbContext dbContext) : IJobsService
{
    public async Task<Result<Job>> CreateAsync(
        JobTemplateName jobTemplateName,
        JobParameters parameters,
        CancellationToken cancellationToken = new())
    {
        var jobTemplate = await dbContext.JobTemplates
            .AsNoTracking()
            .AsSplitQuery()
            .Include(jt => jt.Steps.OrderBy(s => s.Order))
            .Include(jt => jt.Inputs)
            .Include(jt => jt.Outputs)
            .SingleOrDefaultAsync(jt => jt.Name.Equals(jobTemplateName), cancellationToken);

        if (jobTemplate is null)
        {
            return JobTemplateErrors.NotFound(jobTemplateName);
        }

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
}
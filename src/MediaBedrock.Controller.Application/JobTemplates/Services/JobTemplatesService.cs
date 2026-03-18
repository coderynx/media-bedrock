using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Controller.Application.Database;
using MediaBedrock.Controller.Application.JobTemplates.Interfaces;
using MediaBedrock.Controller.Domain.JobTemplates;
using MediaBedrock.Controller.Domain.JobTemplates.Manifests;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Controller.Application.JobTemplates.Services;

public sealed class JobTemplatesService(IControllerDbContext dbContext) : IJobTemplatesService
{
    public async Task<Result<JobTemplate>> CreateAsync(
        JobTemplateManifest manifest,
        CancellationToken cancellationToken = new())
    {
        var template = manifest.ToTemplate();

        var doesExist = await dbContext.JobTemplates
            .AsNoTracking()
            .AnyAsync(j => j.Name.Equals(template.Name) &&
                           j.Version.Equals(template.Version), cancellationToken);

        if (doesExist)
        {
            return JobTemplateErrors.Conflict(template.Name, template.Version);
        }

        await dbContext.JobTemplates.AddAsync(template, cancellationToken);

        return await dbContext.SaveChangesAsync(cancellationToken) is 0
            ? JobTemplateErrors.StoreFailed(template.Name)
            : Result.Created(template);
    }

    public async Task<Option<JobTemplate>> GetAsync(JobTemplateName name, CancellationToken cancellationToken = new())
    {
        var template = await dbContext.JobTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Name.Equals(name), cancellationToken);

        return template is null
            ? Option.None<JobTemplate>()
            : Option.Some(template);
    }

    public async Task<Option<JobTemplate>> GetAsync(
        JobTemplateName name,
        JobTemplateVersion version,
        CancellationToken cancellationToken = new())
    {
        var template = await dbContext.JobTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Name.Equals(name) && j.Version.Equals(version), cancellationToken);

        return template is null
            ? Option.None<JobTemplate>()
            : Option.Some(template);
    }
}
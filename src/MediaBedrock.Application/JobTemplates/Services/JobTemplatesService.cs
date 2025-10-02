using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Application.Database;
using MediaBedrock.Application.JobTemplates.Interfaces;
using MediaBedrock.Domain.JobTemplates;
using MediaBedrock.Domain.JobTemplates.Manifests;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Application.JobTemplates.Services;

public sealed class JobTemplatesService(IApplicationDbContext dbContext) : IJobTemplatesService
{
    public async Task<Result<JobTemplate>> CreateAsync(
        JobTemplateManifest manifest,
        CancellationToken cancellationToken = new())
    {
        var template = manifest.ToTemplate();

        var doesExist = await dbContext.JobTemplates
            .AsNoTracking()
            .AnyAsync(j => j.Name.Equals(template.Name) && j.Version.Equals(template.Version), cancellationToken);

        if (doesExist)
        {
            return Result.Created(template);
        }

        await dbContext.JobTemplates.AddAsync(template, cancellationToken);

        return await dbContext.SaveChangesAsync(cancellationToken) is 0
            ? JobTemplateErrors.StoreFailed(template.Name)
            : Result.Created(template);
    }

    public async Task<Option<JobTemplate>> GetAsync(string name, CancellationToken cancellationToken = new())
    {
        var createJobTemplateName = JobTemplateName.Create(name);
        if (createJobTemplateName.IsFailure)
        {
            return Option.None<JobTemplate>();
        }

        var template = await dbContext.JobTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Name.Equals(createJobTemplateName.Value), cancellationToken);

        return template is null
            ? Option.None<JobTemplate>()
            : Option.Some(template);
    }
}
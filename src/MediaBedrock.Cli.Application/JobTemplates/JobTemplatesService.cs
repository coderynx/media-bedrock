using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Application.JobTemplates.Interfaces;
using MediaBedrock.Cli.Application.Persistence;
using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Domain.JobTemplates.Manifests;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Cli.Application.JobTemplates;

public sealed class JobTemplatesService(IApplicationDbContext dbContext) : IJobTemplatesService
{
    public async Task<Result<JobTemplate>> CreateAsync(
        JobTemplateManifest manifest,
        CancellationToken cancellationToken = default)
    {
        var template = manifest.ToTemplate();

        var doesExist = await dbContext.JobTemplates.AnyAsync(
            predicate: j => j.Name.Equals(template.Name) && j.Version.Equals(template.Version),
            cancellationToken: cancellationToken);

        if (doesExist)
        {
            return Result.Created(template);
        }

        await dbContext.JobTemplates.AddAsync(template, cancellationToken);

        return await dbContext.SaveChangesAsync(cancellationToken) is 0
            ? JobTemplateErrors.StoreFailed(template.Name)
            : Result.Created(template);
    }

    public async Task<Option<JobTemplate>> GetAsync(string name, CancellationToken cancellationToken = default)
    {
        var template = await dbContext.JobTemplates.FirstOrDefaultAsync(
            predicate: j => j.Name.Equals(name),
            cancellationToken: cancellationToken);

        return template is null
            ? Option.None<JobTemplate>()
            : Option.Some(template);
    }
}
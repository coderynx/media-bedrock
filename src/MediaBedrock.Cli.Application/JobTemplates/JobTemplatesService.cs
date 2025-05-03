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
    public async Task<Result<JobTemplate>> CreateAsync(JobTemplateManifest manifest)
    {
        var template = manifest.ToTemplate();

        var doesExist = await dbContext.JobTemplates
            .AnyAsync(j => j.Name.Equals(template.Name) && j.Version.Equals(template.Version));

        if (doesExist)
        {
            return Result.Created(template);
        }

        await dbContext.JobTemplates.AddAsync(template);

        var result = await dbContext.SaveChangesAsync();

        return result is 0
            ? JobTemplateErrors.StoreFailed(template.Name)
            : Result.Created(template);
    }

    public async Task<Option<JobTemplate>> GetAsync(string name)
    {
        var template = await dbContext.JobTemplates.FirstOrDefaultAsync(j => j.Name.Equals(name));

        return template is null
            ? Option<JobTemplate>.None()
            : Option<JobTemplate>.Some(template);
    }
}
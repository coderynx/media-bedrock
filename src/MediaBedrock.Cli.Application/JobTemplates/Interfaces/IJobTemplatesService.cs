using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Domain.JobTemplates.Manifests;

namespace MediaBedrock.Cli.Application.JobTemplates.Interfaces;

public interface IJobTemplatesService
{
    Task<Result<JobTemplate>> CreateAsync(JobTemplateManifest manifest, CancellationToken cancellationToken = default);
    Task<Option<JobTemplate>> GetAsync(string name, CancellationToken cancellationToken = default);
}
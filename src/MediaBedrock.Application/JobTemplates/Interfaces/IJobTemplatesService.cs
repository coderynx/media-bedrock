using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobTemplates;
using MediaBedrock.Domain.JobTemplates.Manifests;

namespace MediaBedrock.Application.JobTemplates.Interfaces;

public interface IJobTemplatesService
{
    Task<Result<JobTemplate>> CreateAsync(JobTemplateManifest manifest, CancellationToken cancellationToken = default);
    Task<Option<JobTemplate>> GetAsync(string name, CancellationToken cancellationToken = default);
}
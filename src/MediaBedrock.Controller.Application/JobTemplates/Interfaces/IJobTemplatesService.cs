using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.JobTemplates;
using MediaBedrock.Controller.Domain.JobTemplates.Manifests;

namespace MediaBedrock.Controller.Application.JobTemplates.Interfaces;

public interface IJobTemplatesService
{
    Task<Result<JobTemplate>> CreateAsync(JobTemplateManifest manifest, CancellationToken cancellationToken = new());
    Task<Option<JobTemplate>> GetAsync(JobTemplateName name, CancellationToken cancellationToken = new());

    Task<Option<JobTemplate>> GetAsync(
        JobTemplateName name,
        JobTemplateVersion version,
        CancellationToken cancellationToken = new());
}
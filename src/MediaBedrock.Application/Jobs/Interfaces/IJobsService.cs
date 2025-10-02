using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Parameters;
using MediaBedrock.Domain.JobTemplates;

namespace MediaBedrock.Application.Jobs.Interfaces;

public interface IJobsService
{
    Task<Option<Job>> GetAsync(JobId id, CancellationToken cancellationToken = default);

    Task<Result<Job>> CreateAsync(
        JobTemplate jobTemplate,
        JobParameters parameters,
        CancellationToken cancellationToken = default);

    Task<Result<JobRunId>> StartAsync(JobId jobId, CancellationToken cancellationToken = default);
    Task<Result<List<JobRunId>>> StartAsync(List<JobId> jobIds, CancellationToken cancellationToken = default);

    Task<List<Job>> GetAsync(CancellationToken cancellationToken = default);
}
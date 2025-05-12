using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Parameters;
using MediaBedrock.Domain.JobStateMachine;
using MediaBedrock.Domain.JobTemplates;

namespace MediaBedrock.Application.Jobs.Interfaces;

public interface IJobsService
{
    Task<Option<Job>> GetAsync(JobId id, CancellationToken cancellationToken = default);

    Task<Result<Job>> CreateAsync(
        JobTemplate jobTemplate,
        JobParameters parameters,
        CancellationToken cancellationToken = default);

    Task<Result<JobStateMachineId>> StartAsync(JobId jobId, CancellationToken cancellationToken = default);
    Task<Result<List<JobStateMachineId>>> StartAsync(List<JobId> jobIds, CancellationToken cancellationToken = default);

    Task<Result> WaitForCompletionAsync(
        JobStateMachineId stateMachineId,
        TimeSpan delayTime,
        CancellationToken cancellationToken = default);

    Task<List<Job>> GetAsync(CancellationToken cancellationToken = default);
}
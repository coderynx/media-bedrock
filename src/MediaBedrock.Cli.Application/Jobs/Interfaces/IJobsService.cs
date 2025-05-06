using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.JobsStateMachine;
using MediaBedrock.Cli.Domain.JobTemplates;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

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
}
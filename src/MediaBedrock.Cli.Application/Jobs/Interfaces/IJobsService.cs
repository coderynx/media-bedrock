using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.JobsStateMachine;
using MediaBedrock.Cli.Domain.JobTemplates;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IJobsService
{
    Task<Result<Job>> CreateAsync(JobTemplate jobTemplate, JobParameters parameters);
    Task<Result<JobStateMachineId>> StartAsync(JobId jobId, CancellationToken ct = default);
    Task<Result<List<JobStateMachineId>>> StartAsync(List<JobId> jobIds, CancellationToken ct = default);

    Task<Result> WaitForCompletionAsync(
        JobStateMachineId stateMachineId,
        TimeSpan delayTime,
        CancellationToken ct = default);
}
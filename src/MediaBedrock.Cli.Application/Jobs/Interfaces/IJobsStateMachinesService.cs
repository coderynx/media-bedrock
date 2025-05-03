using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.JobsStateMachine;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IJobsStateMachinesService
{
    Task<Result> StartJobAsync(JobStateMachineId jobStateMachineId, CancellationToken cancellationToken = default);

    Task<Result> CompleteJobAsync(JobStateMachineId jobStateMachineId, CancellationToken cancellationToken = default);

    Task<Result> StartStepAsync(
        JobStateMachineId jobStateMachineId,
        JobStepStateMachineId jobStepStateMachineId,
        CancellationToken cancellationToken = default);

    Task<Result> CompleteStepAsync(
        JobStepStateMachineId jobStepStateMachineId,
        List<JobAssetName> updatedAssetNames,
        CancellationToken cancellationToken = default);

    Task<Result> FailStepAsync(
        JobStepStateMachineId jobStepStateMachineId,
        JobStepFailureReason reason,
        string message = "",
        CancellationToken cancellationToken = default);
}
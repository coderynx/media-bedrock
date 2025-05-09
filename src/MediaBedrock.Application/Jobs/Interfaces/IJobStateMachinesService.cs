using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.JobStateMachine;
using MediaBedrock.Domain.JobTemplates;

namespace MediaBedrock.Application.Jobs.Interfaces;

public interface IJobStateMachinesService
{
    Task<Result> StartAsync(JobStateMachineId jobStateMachineId, CancellationToken cancellationToken = default);

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

    Task<Option<JobStateMachine>> GetAsync(JobId jobId, CancellationToken ct = default);
    Task<List<JobStateMachine>> GetAsync(JobTemplateName jobTemplateName, CancellationToken ct = default);

    Task DeleteAsync(
        JobTemplateName jobTemplateName,
        JobExecutionStatus jobExecutionStatus = JobExecutionStatus.Pending,
        CancellationToken cancellationToken = default);

    Task<Result> FailJobAsync(JobStateMachineId jobStateMachineId,
        JobFailureReason failureReason,
        string message,
        CancellationToken cancellationToken);
}
using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.JobTemplates;

namespace MediaBedrock.Application.Jobs.Interfaces;

public interface IJobRunService
{
    Task<List<JobRun>> GetAsync(JobTemplateName jobTemplateName, CancellationToken ct = default);
    
    Task<Result> StartAsync(JobRunId jobRunId, CancellationToken cancellationToken = default);

    Task<Result> CompleteJobAsync(JobRunId jobRunId, CancellationToken cancellationToken = default);

    Task<Result> StartStepAsync(
        JobRunId jobRunId,
        JobRunStepId jobRunStepId,
        CancellationToken cancellationToken = default);

    Task<Result> CompleteStepAsync(
        JobRunStepId jobRunStepId,
        List<JobAssetName> updatedAssetNames,
        CancellationToken cancellationToken = default);

    Task<Result> FailStepAsync(
        JobRunStepId jobRunStepId,
        JobStepFailureReason reason,
        string message = "",
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        JobTemplateName jobTemplateName,
        JobRunStatus jobRunStatus = JobRunStatus.Pending,
        CancellationToken cancellationToken = default);

    Task<Result> FailJobAsync(JobRunId jobRunId,
        JobFailureReason failureReason,
        string message,
        CancellationToken cancellationToken);
}
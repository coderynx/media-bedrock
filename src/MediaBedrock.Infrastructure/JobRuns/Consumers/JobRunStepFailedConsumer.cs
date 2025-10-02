using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class JobRunStepFailedConsumer(IJobRunStepsOrchestrator jobRunStepsOrchestrator)
    : IMessageConsumer<JobRunStepFailed>
{
    public async Task HandleAsync(JobRunStepFailed message, CancellationToken cancellationToken = default)
    {
        var createJobRunStepId = JobRunStepId.Create(message.JobRunStepId);
        if (createJobRunStepId.IsFailure)
        {
            return;
        }

        var createFailureReason = Enum.TryParse<JobStepFailureReason>(message.FailureReason, out var failureReason);
        if (!createFailureReason)
        {
            return;
        }

        var failStep = await jobRunStepsOrchestrator.FailAsync(
            jobRunStepId: createJobRunStepId.Value,
            reason: failureReason,
            message: message.Message,
            cancellationToken: cancellationToken);

        if (failStep.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to stop job step {message.JobRunStepId} for job {message.JobRunId}");
        }
    }
}
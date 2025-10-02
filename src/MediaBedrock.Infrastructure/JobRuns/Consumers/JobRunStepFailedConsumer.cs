using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.JobRuns.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class JobRunStepFailedConsumer(IJobRunStepsOrchestrator jobRunStepsOrchestrator)
    : IMessageConsumer<JobRunStepFailed>
{
    public async Task HandleAsync(JobRunStepFailed message, CancellationToken cancellationToken = default)
    {
        var failStep = await jobRunStepsOrchestrator.FailAsync(
            jobRunStepId: message.JobRunStepId,
            reason: message.FailureReason,
            message: message.Message,
            cancellationToken: cancellationToken);

        if (failStep.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to stop job step {message.JobRunStepId} for job {message.JobRunId}");
        }
    }
}
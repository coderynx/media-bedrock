using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Controller.Application.JobRuns.Interfaces;
using MediaBedrock.Controller.Domain.JobRuns;

namespace MediaBedrock.Controller.Infrastructure.JobRuns.Consumers;

public sealed class JobRunStepFailedConsumer(IJobRunStepsOrchestrator jobRunStepsOrchestrator)
    : IConsumer<JobRunStepFailedIntegrationEvent>
{
    public async Task ConsumeAsync(ConsumerContext<JobRunStepFailedIntegrationEvent> context, CancellationToken ct = new())
    {
        var message = context.Message;

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
            cancellationToken: ct);

        if (failStep.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to stop job step {message.JobRunStepId} for job {message.JobRunId}");
        }
    }
}
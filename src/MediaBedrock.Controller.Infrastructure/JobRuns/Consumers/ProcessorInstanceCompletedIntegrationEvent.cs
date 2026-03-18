using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Contracts.Processing;
using MediaBedrock.Controller.Application.JobRuns.Interfaces;
using MediaBedrock.Controller.Domain.JobRuns;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Controller.Infrastructure.JobRuns.Consumers;

public sealed class ProcessorInstanceCompletedIntegrationEventConsumer(
    IJobRunStepsOrchestrator jobRunStepsOrchestrator,
    ILogger<ProcessorInstanceCompletedIntegrationEventConsumer> logger)
    : IConsumer<ProcessorInstanceCompletedIntegrationEvent>
{
    public async Task ConsumeAsync(
        ConsumerContext<ProcessorInstanceCompletedIntegrationEvent> context,
        CancellationToken ct = new())
    {
        var message = context.Message;

        var createJobRunStepId = JobRunStepId.Create(message.JobRunStepId);
        if (createJobRunStepId.IsFailure)
        {
            logger.LogError("Invalid job run step id: {JobRunStepId}", message.JobRunStepId);
            return;
        }

        var jobRunStepId = createJobRunStepId.Value;

        var complete = await jobRunStepsOrchestrator.CompleteAsync(jobRunStepId, ct);
        if (complete.IsFailure)
        {
            logger.LogError("Failed to complete job run step {JobRunStepId}", jobRunStepId);
        }
    }
}
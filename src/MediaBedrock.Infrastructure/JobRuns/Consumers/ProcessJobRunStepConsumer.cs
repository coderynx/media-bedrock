using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Domain.JobRuns;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class ProcessJobRunStepConsumer(
    IJobRunStepsOrchestrator jobRunStepsOrchestrator,
    ILogger<ProcessJobRunStepConsumer> logger)
    : IConsumer<ProcessJobRunStep>
{
    public async Task ConsumeAsync(ConsumerContext<ProcessJobRunStep> context, CancellationToken ct = new())
    {
        var message = context.Message;

        var createJobRunId = JobRunId.Create(message.JobRunId);
        if (createJobRunId.IsFailure)
        {
            logger.LogError("Failed to create job run id for job {JobId}", message.JobRunId);
            return;
        }

        var createJobRunStepId = JobRunStepId.Create(message.JobRunStepId);
        if (createJobRunStepId.IsFailure)
        {
            logger.LogError("Failed to create job run step id for job {JobId}", message.JobRunId);
            return;
        }

        var startStep = await jobRunStepsOrchestrator.StartAsync(
            jobRunId: createJobRunId.Value,
            jobRunStepId: createJobRunStepId.Value,
            cancellationToken: ct);

        if (startStep.IsFailure)
        {
            logger.LogError("Failed to start job run step {JobRunStepId} for job run {JobRunId}: {Error}",
                message.JobRunStepId,
                message.JobRunId,
                startStep.Error);
        }
    }
}
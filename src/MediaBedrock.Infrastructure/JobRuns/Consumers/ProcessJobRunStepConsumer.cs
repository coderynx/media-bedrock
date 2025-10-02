using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class ProcessJobRunStepConsumer(IJobRunStepsOrchestrator jobRunStepsOrchestrator)
    : IConsumer<ProcessJobRunStep>
{
    public async Task ConsumeAsync(ConsumerContext<ProcessJobRunStep> context, CancellationToken ct = new())
    {
        var message = context.Message;

        var createJobRunId = JobRunId.Create(message.JobRunId);
        if (createJobRunId.IsFailure)
        {
            return;
        }

        var createJobRunStepId = JobRunStepId.Create(message.JobRunStepId);
        if (createJobRunStepId.IsFailure)
        {
            return;
        }

        var startStep = await jobRunStepsOrchestrator.StartAsync(
            jobRunId: createJobRunId.Value,
            jobRunStepId: createJobRunStepId.Value,
            cancellationToken: ct);

        if (startStep.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to run job step {message.JobRunStepId} for job {message.JobRunId}");
        }
    }
}
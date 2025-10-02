using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class ProcessStepConsumer(IJobRunStepsOrchestrator jobRunStepsOrchestrator)
    : IMessageConsumer<ProcessJobStep>
{
    public async Task HandleAsync(ProcessJobStep message, CancellationToken cancellationToken = default)
    {
        var startStep = await jobRunStepsOrchestrator.StartAsync(
            jobRunId: message.JobRunId,
            jobRunStepId: message.JobRunStepId,
            cancellationToken: cancellationToken);

        if (startStep.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to run job step {message.JobRunStepId} for job {message.JobRunId}");
        }
    }
}
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Application.JobRuns.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class ProcessJobRunStepConsumer(IJobRunStepsOrchestrator jobRunStepsOrchestrator)
    : IMessageConsumer<ProcessJobRunStep>
{
    public async Task HandleAsync(ProcessJobRunStep message, CancellationToken cancellationToken = default)
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
using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class ProcessStepConsumer(IJobRunService jobRunService)
    : IMessageConsumer<ProcessJobStep>
{
    public async Task HandleAsync(ProcessJobStep message, CancellationToken cancellationToken = default)
    {
        var startStep = await jobRunService.StartStepAsync(
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
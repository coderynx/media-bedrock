using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class JobStepFailedConsumer(IJobStateMachinesService jobStateMachinesService)
    : IMessageConsumer<JobStepFailed>
{
    public async Task HandleAsync(JobStepFailed message, CancellationToken cancellationToken = default)
    {
        var stop = await jobStateMachinesService.FailStepAsync(
            jobStepStateMachineId: message.JobStepStateMachineId,
            reason: message.FailureReason,
            message: message.Message,
            cancellationToken: cancellationToken);

        if (stop.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to stop job step {message.JobStepStateMachineId} for job {message.JobStateMachineId}");
        }
    }
}
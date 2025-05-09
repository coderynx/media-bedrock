using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class JobStepCompletedConsumer(IJobStateMachinesService jobStateMachinesService)
    : IMessageConsumer<JobStepCompleted>
{
    public async Task HandleAsync(JobStepCompleted message, CancellationToken cancellationToken = default)
    {
        var stop = await jobStateMachinesService.CompleteStepAsync(
            jobStepStateMachineId: message.JobStepStateMachineId,
            updatedAssetNames: message.UpdatedAssetNames,
            cancellationToken: cancellationToken);

        if (stop.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to stop job step {message.JobStepStateMachineId} for job {message.JobStateMachineId}");
        }
    }
}
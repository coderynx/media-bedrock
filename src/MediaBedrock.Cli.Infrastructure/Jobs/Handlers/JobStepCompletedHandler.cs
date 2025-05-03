using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Application.Jobs.Messages;
using MediaBedrock.Cli.Infrastructure.Messaging;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Handlers;

public sealed class JobStepCompletedHandler(IJobsStateMachinesService jobsStateMachinesService)
    : IMessageHandler<JobStepCompleted>
{
    public async Task HandleAsync(JobStepCompleted message, CancellationToken ct = default)
    {
        var stop = await jobsStateMachinesService.CompleteStepAsync(
            jobStepStateMachineId: message.JobStepStateMachineId,
            updatedAssetNames: message.UpdatedAssetNames,
            cancellationToken: ct);

        if (stop.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to stop job step {message.JobStepStateMachineId} for job {message.JobStateMachineId}");
        }
    }
}
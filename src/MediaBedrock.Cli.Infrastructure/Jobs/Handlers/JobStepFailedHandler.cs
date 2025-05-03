using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Application.Jobs.Messages;
using MediaBedrock.Cli.Infrastructure.Messaging;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Handlers;

public sealed class JobStepFailedHandler(IJobsStateMachinesService jobsStateMachinesService)
    : IMessageHandler<JobStepFailed>
{
    public async Task HandleAsync(JobStepFailed message, CancellationToken ct = default)
    {
        var stop = await jobsStateMachinesService.FailStepAsync(
            jobStepStateMachineId: message.JobStepStateMachineId,
            reason: message.FailureReason,
            message: message.Message,
            cancellationToken: ct);

        if (stop.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to stop job step {message.JobStepStateMachineId} for job {message.JobStateMachineId}");
        }
    }
}
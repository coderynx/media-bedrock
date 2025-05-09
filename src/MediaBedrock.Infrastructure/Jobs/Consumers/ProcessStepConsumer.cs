using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class ProcessStepConsumer(IJobStateMachinesService jobStateMachinesService)
    : IMessageConsumer<ProcessJobStep>
{
    public async Task HandleAsync(ProcessJobStep message, CancellationToken cancellationToken = default)
    {
        var run = await jobStateMachinesService.StartStepAsync(
            jobStateMachineId: message.JobStateMachineId,
            jobStepStateMachineId: message.JobStepStateMachineId,
            cancellationToken: cancellationToken);

        if (run.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to run job step {message.JobStepStateMachineId} for job {message.JobStateMachineId}");
        }
    }
}
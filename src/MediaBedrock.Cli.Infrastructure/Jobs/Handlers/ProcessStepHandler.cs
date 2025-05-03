using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Application.Jobs.Messages;
using MediaBedrock.Cli.Infrastructure.Messaging;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Handlers;

public sealed class ProcessStepHandler(IJobsStateMachinesService jobsStateMachinesService)
    : IMessageHandler<ProcessJobStep>
{
    public async Task HandleAsync(ProcessJobStep message, CancellationToken ct = default)
    {
        var run = await jobsStateMachinesService.StartStepAsync(message.JobStateMachineId, message.StepStateMachineId,
            ct);
        if (run.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to run job step {message.StepStateMachineId} for job {message.JobStateMachineId}");
        }
    }
}
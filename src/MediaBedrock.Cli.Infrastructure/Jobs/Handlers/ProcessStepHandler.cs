using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Application.Jobs.Messages;
using MediaBedrock.Cli.Infrastructure.Messaging;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Handlers;

public sealed class ProcessStepHandler(IJobsStateMachinesService jobsStateMachinesService)
    : IMessageHandler<ProcessJobStep>
{
    public async Task HandleAsync(ProcessJobStep message, CancellationToken cancellationToken = default)
    {
        var run = await jobsStateMachinesService.StartStepAsync(
            jobStateMachineId: message.JobStateMachineId,
            jobStepStateMachineId: message.StepStateMachineId,
            cancellationToken: cancellationToken);
        
        if (run.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to run job step {message.StepStateMachineId} for job {message.JobStateMachineId}");
        }
    }
}
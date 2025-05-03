using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Application.Jobs.Messages;
using MediaBedrock.Cli.Infrastructure.Messaging;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Handlers;

public sealed class RunHandler(IJobsStateMachinesService jobStateMachineService) : IMessageHandler<RunJob>
{
    public async Task HandleAsync(RunJob message, CancellationToken ct = default)
    {
        var run = await jobStateMachineService.StartJobAsync(message.JobStateMachineId, ct);
        if (run.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to run job {message.JobStateMachineId}");
        }
    }
}
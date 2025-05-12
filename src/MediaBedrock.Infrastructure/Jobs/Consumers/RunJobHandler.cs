using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class RunConsumer(IJobStateMachinesService jobStateMachineService) : IMessageConsumer<RunJob>
{
    public async Task HandleAsync(RunJob message, CancellationToken cancellationToken = default)
    {
        var run = await jobStateMachineService.StartAsync(message.JobStateMachineId, cancellationToken);
        if (run.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to run job {message.JobStateMachineId}");
        }
    }
}
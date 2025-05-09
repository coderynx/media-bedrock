using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class JobCompletedConsumer(IJobStateMachinesService jobStateMachinesService)
    : IMessageConsumer<JobCompleted>
{
    public async Task HandleAsync(JobCompleted message, CancellationToken cancellationToken = default)
    {
        var completeJobs = await jobStateMachinesService.CompleteJobAsync(
            jobStateMachineId: message.JobStateMachineId,
            cancellationToken: cancellationToken);

        if (completeJobs.IsFailure)
        {
            throw new InvalidOperationException($"Failed to complete job {message.JobStateMachineId}");
        }
    }
}
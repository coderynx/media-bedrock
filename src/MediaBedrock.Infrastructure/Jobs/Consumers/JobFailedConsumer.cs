using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class JobFailedConsumer(IJobStateMachinesService jobStateMachinesService) : IMessageConsumer<JobFailed>
{
    public async Task HandleAsync(JobFailed message, CancellationToken cancellationToken = default)
    {
        var failJob = await jobStateMachinesService.FailJobAsync(
            jobStateMachineId: message.JobStateMachineId,
            failureReason: message.FailureReason,
            message: message.Message,
            cancellationToken: cancellationToken);

        if (failJob.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to fail job {message.JobStateMachineId} with reason {message.FailureReason}");
        }
    }
}
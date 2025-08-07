using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Application.Jobs.Messages;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.Jobs.Consumers;

public sealed class JobCompletedConsumer(IJobRunService jobRunService)
    : IMessageConsumer<JobCompleted>
{
    public async Task HandleAsync(JobCompleted message, CancellationToken cancellationToken = default)
    {
        var completeJob = await jobRunService.CompleteJobAsync(
            jobRunId: message.JobRunId,
            cancellationToken: cancellationToken);

        if (completeJob.IsFailure)
        {
            throw new InvalidOperationException($"Failed to complete job {message.JobRunId}");
        }
    }
}
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class JobRunCompletedConsumer(IJobRunsOrchestrator jobRunsOrchestrator) 
    : IMessageConsumer<JobRunCompleted>
{
    public async Task HandleAsync(JobRunCompleted message, CancellationToken cancellationToken = default)
    {
        var createJobRunId = JobRunId.Create(message.JobRunId);
        if (createJobRunId.IsFailure)
        {
            return;
        }
        
        var completeJob = await jobRunsOrchestrator.CompleteAsync(
            jobRunId: createJobRunId.Value,
            cancellationToken: cancellationToken);

        if (completeJob.IsFailure)
        {
            throw new InvalidOperationException($"Failed to complete job {message.JobRunId}");
        }
    }
}
using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class JobRunCompletedConsumer(IJobRunsOrchestrator jobRunsOrchestrator) : IConsumer<JobRunCompleted>
{
    public async Task ConsumeAsync(ConsumerContext<JobRunCompleted> context, CancellationToken ct = new())
    {
        var message = context.Message;

        var createJobRunId = JobRunId.Create(message.JobRunId);
        if (createJobRunId.IsFailure)
        {
            return;
        }

        var completeJob = await jobRunsOrchestrator.CompleteAsync(
            jobRunId: createJobRunId.Value,
            cancellationToken: ct);

        if (completeJob.IsFailure)
        {
            throw new InvalidOperationException($"Failed to complete job {message.JobRunId}");
        }
    }
}
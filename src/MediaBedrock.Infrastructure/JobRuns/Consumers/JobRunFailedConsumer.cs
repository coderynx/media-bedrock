using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class JobRunFailedConsumer(IJobRunsOrchestrator jobRunsOrchestrator) : IConsumer<JobRunFailed>
{
    public async Task ConsumeAsync(ConsumerContext<JobRunFailed> context, CancellationToken ct = new())
    {
        var message = context.Message;

        var createJobRunId = JobRunId.Create(message.JobRunId);
        if (createJobRunId.IsFailure)
        {
            return;
        }

        var parseFailureReason = Enum.TryParse<JobFailureReason>(message.FailureReason, out var failureReason);
        if (!parseFailureReason)
        {
            return;
        }

        var failJob = await jobRunsOrchestrator.FailAsync(
            jobRunId: createJobRunId.Value,
            failureReason: failureReason,
            message: message.Message,
            cancellationToken: ct);

        if (failJob.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to fail job {message.JobRunId} with reason {message.FailureReason}");
        }
    }
}
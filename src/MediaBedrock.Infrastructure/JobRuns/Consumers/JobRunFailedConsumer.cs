using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class JobRunFailedConsumer(IJobRunsOrchestrator jobRunsOrchestrator) : IMessageConsumer<JobRunFailed>
{
    public async Task HandleAsync(JobRunFailed message, CancellationToken cancellationToken = default)
    {
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
            cancellationToken: cancellationToken);

        if (failJob.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to fail job {message.JobRunId} with reason {message.FailureReason}");
        }
    }
}
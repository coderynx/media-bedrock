using MediaBedrock.Application.JobRuns.Interfaces;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Infrastructure.Messaging;

namespace MediaBedrock.Infrastructure.JobRuns.Consumers;

public sealed class ProcessJobRunStepConsumer(IJobRunStepsOrchestrator jobRunStepsOrchestrator)
    : IMessageConsumer<ProcessJobRunStep>
{
    public async Task HandleAsync(ProcessJobRunStep message, CancellationToken cancellationToken = default)
    {
        var createJobRunId = JobRunId.Create(message.JobRunId);
        if (createJobRunId.IsFailure)
        {
            return;
        }

        var createJobRunStepId = JobRunStepId.Create(message.JobRunStepId);
        if (createJobRunStepId.IsFailure)
        {
            return;
        }

        var startStep = await jobRunStepsOrchestrator.StartAsync(
            jobRunId: createJobRunId.Value,
            jobRunStepId: createJobRunStepId.Value,
            cancellationToken: cancellationToken);

        if (startStep.IsFailure)
        {
            throw new InvalidOperationException(
                $"Failed to run job step {message.JobRunStepId} for job {message.JobRunId}");
        }
    }
}
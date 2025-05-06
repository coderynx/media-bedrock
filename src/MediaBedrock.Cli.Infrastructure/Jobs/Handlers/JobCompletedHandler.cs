using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Application.Jobs.Messages;
using MediaBedrock.Cli.Infrastructure.Messaging;

namespace MediaBedrock.Cli.Infrastructure.Jobs.Handlers;

public sealed class JobCompletedHandler(IJobsStateMachinesService jobsStateMachinesService)
    : IMessageHandler<JobCompleted>
{
    public async Task HandleAsync(JobCompleted message, CancellationToken cancellationToken = default)
    {
        var completeJobs = await jobsStateMachinesService.CompleteJobAsync(
            jobStateMachineId: message.JobStateMachineId,
            cancellationToken: cancellationToken);

        if (completeJobs.IsFailure)
        {
            throw new InvalidOperationException($"Failed to complete job {message.JobStateMachineId}");
        }
    }
}
using MediaBedrock.Cli.Domain.JobsStateMachine;

namespace MediaBedrock.Cli.Application.Jobs.Messages;

public sealed record RunJob : JobMessageBase
{
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;

    public static RunJob Create(JobStateMachineId jobStateMachineId)
    {
        var startJob = new RunJob
        {
            JobStateMachineId = jobStateMachineId,
            StartedAt = DateTime.UtcNow
        };

        return startJob;
    }
}
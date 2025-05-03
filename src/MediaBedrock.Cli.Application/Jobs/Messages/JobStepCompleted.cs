using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.JobsStateMachine;

namespace MediaBedrock.Cli.Application.Jobs.Messages;

public sealed record JobStepCompleted : JobMessageBase
{
    public required JobStepStateMachineId JobStepStateMachineId { get; init; }
    public List<JobAssetName> UpdatedAssetNames { get; init; } = [];

    public static JobStepCompleted Create(
        JobStateMachineId jobStateMachineId,
        JobStepStateMachineId jobStepStateMachineId,
        List<JobAssetName> updatedAssetNames)
    {
        return new JobStepCompleted
        {
            JobStateMachineId = jobStateMachineId,
            JobStepStateMachineId = jobStepStateMachineId,
            UpdatedAssetNames = updatedAssetNames
        };
    }
}
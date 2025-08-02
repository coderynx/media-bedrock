using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobStateMachines;

namespace MediaBedrock.Application.Jobs.Messages;

public sealed record JobStepCompleted : JobMessageBase
{
    public JobStepCompleted(
        JobStateMachineId jobStateMachineId,
        JobStepStateMachineId jobStepStateMachineId,
        List<JobAssetName> updatedAssetNames) : base(jobStateMachineId)
    {
        JobStepStateMachineId = jobStepStateMachineId;
        UpdatedAssetNames = updatedAssetNames;
    }

    public JobStepStateMachineId JobStepStateMachineId { get; }
    public List<JobAssetName> UpdatedAssetNames { get; }
}
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.Jobs.Messages;

public sealed record JobStepCompleted : JobMessageBase
{
    public JobStepCompleted(
        JobRunId jobRunId,
        JobRunStepId jobRunStepId,
        List<JobAssetName> updatedAssetNames) : base(jobRunId)
    {
        JobRunStepId = jobRunStepId;
        UpdatedAssetNames = updatedAssetNames;
    }

    public JobRunStepId JobRunStepId { get; }
    public List<JobAssetName> UpdatedAssetNames { get; }
}
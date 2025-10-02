using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.JobRuns.Messages;

public sealed record JobRunStepCompleted : JobRunMessageBase
{
    public JobRunStepCompleted(
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
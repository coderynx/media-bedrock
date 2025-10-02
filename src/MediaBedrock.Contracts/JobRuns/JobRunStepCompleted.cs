namespace MediaBedrock.Contracts.JobRuns;

public sealed record JobRunStepCompleted(
    Guid JobRunId,
    Guid JobRunStepId,
    List<string> UpdatedAssetNames)
    : JobRunMessageBase(JobRunId);
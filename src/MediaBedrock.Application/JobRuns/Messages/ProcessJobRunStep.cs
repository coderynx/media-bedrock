using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.JobRuns.Messages;

public sealed record ProcessJobRunStep : JobRunMessageBase
{
    public ProcessJobRunStep(
        JobRunId jobRunId,
        JobRunStepId jobRunJobRunStepId) : base(jobRunId)
    {
        JobRunStepId = jobRunJobRunStepId;
    }

    public JobRunStepId JobRunStepId { get; }
}
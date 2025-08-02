using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.Jobs.Messages;

public sealed record ProcessJobStep : JobMessageBase
{
    public ProcessJobStep(
        JobRunId jobRunId,
        JobRunStepId jobRunJobRunStepId) : base(jobRunId)
    {
        JobRunStepId = jobRunJobRunStepId;
    }

    public JobRunStepId JobRunStepId { get; }
}
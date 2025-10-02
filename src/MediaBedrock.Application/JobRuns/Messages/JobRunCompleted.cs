using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.JobRuns.Messages;

public sealed record JobRunCompleted : JobRunMessageBase
{
    public JobRunCompleted(JobRunId jobRunId) : base(jobRunId)
    {
    }
}
using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.Jobs.Messages;

public sealed record JobCompleted : JobMessageBase
{
    public JobCompleted(JobRunId jobRunId) : base(jobRunId)
    {
    }
}
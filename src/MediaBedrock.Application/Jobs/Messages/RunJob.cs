using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.Jobs.Messages;

public sealed record RunJob : JobMessageBase
{
    public RunJob(JobRunId jobRunId) : base(jobRunId)
    {
    }
}
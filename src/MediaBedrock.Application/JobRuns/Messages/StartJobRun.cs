using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.JobRuns.Messages;

public sealed record StartJobRun : JobRunMessageBase
{
    public StartJobRun(JobRunId jobRunId) : base(jobRunId)
    {
    }
}
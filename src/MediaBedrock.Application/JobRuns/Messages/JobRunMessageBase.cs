using MediaBedrock.Application.Jobs.Interfaces;
using MediaBedrock.Domain.JobRuns;

namespace MediaBedrock.Application.JobRuns.Messages;

public abstract record JobRunMessageBase : IMessage
{
    protected JobRunMessageBase(JobRunId jobRunId)
    {
        JobRunId = jobRunId;
    }

    public Guid Id { get; init; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public JobRunId JobRunId { get; }
}
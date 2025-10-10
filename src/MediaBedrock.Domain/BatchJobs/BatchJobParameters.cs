using MediaBedrock.Domain.Jobs.Parameters;

namespace MediaBedrock.Domain.BatchJobs;

public sealed class BatchJobParameters
{
    public JobParameters[] Entries { get; init; } = [];
}
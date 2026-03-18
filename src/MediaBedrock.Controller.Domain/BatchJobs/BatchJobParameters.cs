using MediaBedrock.Controller.Domain.Jobs.Parameters;

namespace MediaBedrock.Controller.Domain.BatchJobs;

public sealed class BatchJobParameters
{
    public JobParameters[] Entries { get; init; } = [];
}
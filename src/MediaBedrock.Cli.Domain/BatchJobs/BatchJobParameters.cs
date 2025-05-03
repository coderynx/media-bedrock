using MediaBedrock.Cli.Domain.Jobs.Parameters;

namespace MediaBedrock.Cli.Domain.BatchJobs;

public sealed class BatchJobParameters
{
    public JobParameters[] Entries { get; init; } = [];
}
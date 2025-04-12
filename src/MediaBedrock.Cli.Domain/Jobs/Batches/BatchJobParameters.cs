using MediaBedrock.Cli.Domain.Jobs.Parameters;

namespace MediaBedrock.Cli.Domain.Jobs.Batches;

public sealed class BatchJobParameters
{
    public JobParameters[] Entries { get; init; } = [];
}
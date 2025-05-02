using MediaBedrock.Cli.Presentation.Jobs.Contracts;

namespace MediaBedrock.Cli.Presentation.BatchJobs.Contracts;

public sealed record BatchJobParametersDto
{
    public IReadOnlyList<JobParametersDto> Entries { get; init; } = [];
}
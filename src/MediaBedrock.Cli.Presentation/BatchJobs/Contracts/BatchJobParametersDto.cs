using MediaBedrock.Cli.Presentation.Jobs.Contracts;

namespace MediaBedrock.Cli.Presentation.BatchJobs.Contracts;

public sealed record BatchJobParametersDto
{
    public List<JobParametersDto> Entries { get; init; } = [];
}
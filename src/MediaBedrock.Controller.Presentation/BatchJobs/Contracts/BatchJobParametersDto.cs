using MediaBedrock.Controller.Presentation.Jobs.Contracts;

namespace MediaBedrock.Controller.Presentation.BatchJobs.Contracts;

public sealed record BatchJobParametersDto
{
    public IReadOnlyList<JobParametersDto> Entries { get; init; } = [];
}
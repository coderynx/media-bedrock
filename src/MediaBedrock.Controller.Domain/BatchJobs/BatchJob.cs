using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.Jobs;

namespace MediaBedrock.Controller.Domain.BatchJobs;

public sealed record BatchJob
{
    public required BatchJobId Id { get; init; }
    public IEnumerable<Job> Jobs { get; init; } = [];

    public static Result<BatchJob> Create(IEnumerable<Job> jobs)
    {
        var job = new BatchJob
        {
            Id = BatchJobId.Create(),
            Jobs = jobs
        };

        return Result.Created(job);
    }
}
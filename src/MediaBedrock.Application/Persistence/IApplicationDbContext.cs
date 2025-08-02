using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Application.Persistence;

public interface IApplicationDbContext
{
    DbSet<JobRun> JobRuns { get; }
    DbSet<JobRunStep> JobRunSteps { get; }
    DbSet<Job> Jobs { get; }
    DbSet<JobTemplate> JobTemplates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
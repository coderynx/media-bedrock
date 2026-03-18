using MediaBedrock.Controller.Domain.JobAssets;
using MediaBedrock.Controller.Domain.JobRuns;
using MediaBedrock.Controller.Domain.Jobs;
using MediaBedrock.Controller.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Controller.Application.Database;

public interface IControllerDbContext
{
    DbSet<JobAsset> JobAssets { get; }
    DbSet<JobRun> JobRuns { get; }
    DbSet<JobRunStep> JobRunSteps { get; }
    DbSet<Job> Jobs { get; }
    DbSet<JobTemplate> JobTemplates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
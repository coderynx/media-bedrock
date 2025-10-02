using MediaBedrock.Application.Database;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Infrastructure.Database;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<JobRun> JobRuns { get; init; }
    public DbSet<JobRunStep> JobRunSteps { get; init; }
    public DbSet<Job> Jobs { get; init; }
    public DbSet<JobTemplate> JobTemplates { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
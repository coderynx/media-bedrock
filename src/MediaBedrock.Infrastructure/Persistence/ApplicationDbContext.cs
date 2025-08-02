using MediaBedrock.Application.Persistence;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.JobStateMachines;
using MediaBedrock.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<JobStateMachine> JobStateMachines { get; init; }
    public DbSet<JobStepStateMachine> JobStepStateMachines { get; init; }
    public DbSet<Job> Jobs { get; init; }
    public DbSet<JobTemplate> JobTemplates { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
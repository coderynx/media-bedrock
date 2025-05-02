using MediaBedrock.Cli.Application.Persistence;
using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Cli.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<JobStateMachine> JobsStateMachines { get; init; }
    public DbSet<JobStepStateMachine> JobStepStateMachines { get; init; }
    public DbSet<Job> Jobs { get; init; }
    public DbSet<JobStep> JobSteps { get; init; }
    public DbSet<JobTemplate> JobTemplates { get; init; }
    public DbSet<JobTemplateStep> JobTemplateSteps { get; init; }
    public DbSet<JobAsset> JobAssets { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
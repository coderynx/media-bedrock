using MediaBedrock.Controller.Application.Database;
using MediaBedrock.Controller.Domain.JobAssets;
using MediaBedrock.Controller.Domain.JobRuns;
using MediaBedrock.Controller.Domain.Jobs;
using MediaBedrock.Controller.Domain.JobTemplates;
using MediaBedrock.Core.Domain.Abstractions;
using MediaBedrock.Core.Infrastructure.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Controller.Infrastructure.Database;

internal sealed class ControllerDbContext(
    DbContextOptions<ControllerDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher)
    : DbContext(options), IControllerDbContext
{
    public DbSet<JobAsset> JobAssets { get; init; }
    public DbSet<JobRun> JobRuns { get; init; }
    public DbSet<JobRunStep> JobRunSteps { get; init; }
    public DbSet<Job> Jobs { get; init; }
    public DbSet<JobTemplate> JobTemplates { get; init; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ControllerDbContext).Assembly);
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        await domainEventsDispatcher.DispatchAsync(domainEvents);
    }
}
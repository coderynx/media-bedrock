using MediaBedrock.Core.Domain.Abstractions;
using MediaBedrock.Core.Infrastructure.DomainEvents;
using MediaBedrock.Worker.Application.Database;
using MediaBedrock.Worker.Domain.Processing.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Worker.Infrastructure.Database;

internal sealed class WorkerDbContext(
    DbContextOptions<WorkerDbContext> options,
    IDomainEventsDispatcher domainEventsDispatcher) 
    : DbContext(options), IWorkerDbContext
{
    public DbSet<ProcessorInstance> ProcessorInstances { get; init; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync();

        return result;
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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkerDbContext).Assembly);
    }
}
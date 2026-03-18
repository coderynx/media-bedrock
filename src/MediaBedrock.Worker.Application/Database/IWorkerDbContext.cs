using MediaBedrock.Worker.Domain.Processing.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Worker.Application.Database;

public interface IWorkerDbContext
{
    DbSet<ProcessorInstance> ProcessorInstances { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());
}
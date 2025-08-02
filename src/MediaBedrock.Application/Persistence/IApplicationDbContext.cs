using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.JobStateMachines;
using MediaBedrock.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Application.Persistence;

public interface IApplicationDbContext
{
    DbSet<JobStateMachine> JobStateMachines { get; }
    DbSet<JobStepStateMachine> JobStepStateMachines { get; }
    DbSet<Job> Jobs { get; }
    DbSet<JobTemplate> JobTemplates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
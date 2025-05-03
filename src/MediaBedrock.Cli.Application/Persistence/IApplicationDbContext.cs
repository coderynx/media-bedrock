using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.JobsStateMachine;
using MediaBedrock.Cli.Domain.JobTemplates;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Cli.Application.Persistence;

public interface IApplicationDbContext
{
    DbSet<JobStateMachine> JobsStateMachines { get; init; }
    DbSet<JobStepStateMachine> JobStepStateMachines { get; init; }
    DbSet<Job> Jobs { get; init; }
    DbSet<JobStep> JobSteps { get; init; }
    DbSet<JobTemplate> JobTemplates { get; init; }
    DbSet<JobTemplateStep> JobTemplateSteps { get; init; }
    DbSet<JobAsset> JobAssets { get; init; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
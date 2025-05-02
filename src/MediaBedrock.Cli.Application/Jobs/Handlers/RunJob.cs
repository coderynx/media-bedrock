using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Application.Persistence;
using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.Jobs;
using Microsoft.EntityFrameworkCore;

namespace MediaBedrock.Cli.Application.Jobs.Handlers;

public sealed record RunJob : JobMessage
{
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;

    public static RunJob Create(JobStateMachine jobStateMachine)
    {
        var startJob = new RunJob
        {
            JobId = jobStateMachine.Job.Id,
            StartedAt = DateTime.UtcNow
        };

        return startJob;
    }
}

public sealed class RunJobHandler(
    IJobMessageBus messageBus,
    IApplicationDbContext dbContext) : IJobMessageHandler<RunJob>
{
    public async Task HandleAsync(RunJob message, CancellationToken ct = default)
    {
        var jobStateMachine = await dbContext.JobsStateMachines
            .Include(j => j.AssetsPool)
            .Include(j => j.Job)
            .Include(j => j.StepsStateMachines)
            .ThenInclude(s => s.StepSinks)
            .Include(j => j.StepsStateMachines)
            .ThenInclude(s => s.StepSources)
            .AsNoTracking()
            .SingleOrDefaultAsync(j => j.Job.Id.Equals(message.JobId), ct);

        if (jobStateMachine is null)
        {
            throw new InvalidOperationException($"Job state machine not found for job {message.JobId}");
        }

        var inputAssets = jobStateMachine.ResolveAssets(JobAssetKind.Input);

        var jobSteps = jobStateMachine.StepsStateMachines
            .Where(s => s.StepSinks.Any(a => inputAssets.Any(i => i.Name.Equals(a.AssetName))))
            .ToList();

        jobStateMachine.TransitionToRunning();
        await dbContext.SaveChangesAsync(ct);

        var processJobSteps = jobSteps.Select(jobStep => new ProcessJobStep
        {
            JobId = jobStateMachine.Job.Id,
            StepName = jobStep.StepName
        });

        foreach (var processJobStep in processJobSteps) await messageBus.PublishAsync(processJobStep, ct);
    }
}
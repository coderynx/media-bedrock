using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Application.Database;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Domain.Abstractions;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobRuns.DomainEvents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Application.JobRuns.DomainEventConsumers;

public sealed class JobRunStartedDomainEventConsumer(
    IApplicationDbContext dbContext,
    IMessagePublisher messagePublisher,
    ILogger<JobRunStartedDomainEventConsumer> logger)
    : IDomainEventConsumer<JobRunStartedDomainEvent>
{
    public async Task ConsumeAsync(JobRunStartedDomainEvent domainEvent, CancellationToken cancellationToken = new())
    {
        var jobRun = await dbContext.JobRuns
            .AsSplitQuery()
            .AsNoTracking()
            .Include(j => j.AssetsPool)
            .Include(j => j.Steps)
            .ThenInclude(s => s.StepInputs)
            .Include(j => j.Steps)
            .ThenInclude(s => s.StepOutputs)
            .SingleOrDefaultAsync(j => j.Id.Equals(domainEvent.JobRunId), cancellationToken);

        if (jobRun is null)
        {
            logger.LogError(
                "JobRun {JobRunId} was not found when processing JobRunStartedDomainEvent",
                domainEvent.JobRunId);

            return;
        }

        var inputAssets = jobRun.ResolveAssets(JobAssetKind.Input);

        var processJobSteps = jobRun.Steps
            .Where(s => s.StepInputs.Any(a => inputAssets.Any(i => i.Name.Equals(a.AssetName))))
            .Select(jobStep => new ProcessJobRunStep(jobRun.Id.Value, jobStep.Id.Value))
            .ToList();

        foreach (var processJobStep in processJobSteps)
        {
            await messagePublisher.PublishAsync(processJobStep, cancellationToken);
        }
    }
}
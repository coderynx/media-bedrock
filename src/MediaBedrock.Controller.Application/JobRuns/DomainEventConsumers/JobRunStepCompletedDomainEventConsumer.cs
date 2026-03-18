using MediaBedrock.Controller.Application.Database;
using MediaBedrock.Controller.Domain.JobRuns.DomainEvents;
using MediaBedrock.Core.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Controller.Application.JobRuns.DomainEventConsumers;

public sealed class JobRunStepCompletedDomainEventConsumer(
    IControllerDbContext dbContext,
    ILogger<JobRunStepCompletedDomainEventConsumer> logger) : IDomainEventConsumer<JobRunStepCompletedDomainEvent>
{
    public async Task ConsumeAsync(
        JobRunStepCompletedDomainEvent domainEvent,
        CancellationToken cancellationToken = new())
    {
        var jobRun = await dbContext.JobRuns
            .AsSplitQuery()
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

        var startReadyToRunSteps = jobRun.Advance();
        if (startReadyToRunSteps.IsFailure)
        {
            logger.LogError(
                "Failed to start input steps for job run {JobRunId}",
                domainEvent.JobRunId);
        }
    }
}
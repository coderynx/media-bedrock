using MediaBedrock.Core.Domain.Abstractions;
using MediaBedrock.Worker.Application.Processing.Interfaces;
using MediaBedrock.Worker.Domain.Processing.DomainEvents;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Worker.Application.Processing.DomainEvents;

public sealed class ProcessorInstanceStartedDomainEventConsumer(
    IProcessorRunner processorRunner,
    ILogger<ProcessorInstanceStartedDomainEventConsumer> logger)
    : IDomainEventConsumer<ProcessorInstanceStartedDomainEvent>
{
    public async Task ConsumeAsync(
        ProcessorInstanceStartedDomainEvent domainEvent,
        CancellationToken cancellationToken = new())
    {
        var runResult = await processorRunner.RunAsync(domainEvent.ProcessorInstanceId, cancellationToken);
        if (runResult.IsFailure)
        {
            logger.LogError("Failed to run processor instance {ProcessorInstanceId}", domainEvent.ProcessorInstanceId);
        }
    }
}
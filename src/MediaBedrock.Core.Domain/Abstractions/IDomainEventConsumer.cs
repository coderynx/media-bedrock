namespace MediaBedrock.Core.Domain.Abstractions;

public interface IDomainEventConsumer<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task ConsumeAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = new());
}
using MediaBedrock.Domain.Abstractions;

namespace MediaBedrock.Infrastructure.DomainEvents;

internal interface IDomainEventsDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = new());
}
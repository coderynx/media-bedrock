using MediaBedrock.Core.Domain.Abstractions;

namespace MediaBedrock.Core.Infrastructure.DomainEvents;

public interface IDomainEventsDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = new());
}
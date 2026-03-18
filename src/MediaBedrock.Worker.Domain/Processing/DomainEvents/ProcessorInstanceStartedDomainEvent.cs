using MediaBedrock.Core.Domain.Abstractions;
using MediaBedrock.Worker.Domain.Processing.ValueObjects;

namespace MediaBedrock.Worker.Domain.Processing.DomainEvents;

public sealed record ProcessorInstanceStartedDomainEvent(ProcessorInstanceId ProcessorInstanceId) : IDomainEvent; 

using MediaBedrock.Domain.Abstractions;

namespace MediaBedrock.Domain.JobRuns.DomainEvents;

public sealed record JobRunStartedDomainEvent(JobRunId JobRunId) : IDomainEvent;
using MediaBedrock.Core.Domain.Abstractions;

namespace MediaBedrock.Controller.Domain.JobRuns.DomainEvents;

public sealed record JobRunStartedDomainEvent(JobRunId JobRunId) : IDomainEvent;
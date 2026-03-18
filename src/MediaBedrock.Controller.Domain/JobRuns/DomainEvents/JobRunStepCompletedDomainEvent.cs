using MediaBedrock.Core.Domain.Abstractions;

namespace MediaBedrock.Controller.Domain.JobRuns.DomainEvents;

public sealed record JobRunStepCompletedDomainEvent(JobRunId JobRunId, JobRunStepId JobRunStepId) : IDomainEvent;

using System.Collections.Concurrent;
using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

public sealed class InMemoryJobStateMachineRepository : IJobStateMachineRepository
{
    private readonly ConcurrentDictionary<JobId, JobStateMachine> _jobContainers = new();

    public Result Store(JobStateMachine jobStateMachine)
    {
        return _jobContainers.TryAdd(jobStateMachine.JobId, jobStateMachine)
            ? Result.Created()
            : JobErrors.ContainerConflict(jobStateMachine.JobId);
    }

    public Option<JobStateMachine> Get(JobId jobId)
    {
        return _jobContainers.TryGetValue(jobId, out var jobContainer)
            ? Option<JobStateMachine>.Some(jobContainer)
            : Option<JobStateMachine>.None();
    }

    public Result Remove(JobId jobId)
    {
        if (!_jobContainers.TryRemove(jobId, out var jobContainer))
        {
            return JobErrors.ContainerRemovalFailed(jobId);
        }

        jobContainer.Dispose();
        return Result.Deleted();
    }
}
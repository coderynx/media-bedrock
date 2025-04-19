using Coderynx.Functional.Options;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Interfaces;

/// <summary>
///     Interface for storing and retrieving job containers.
/// </summary>
public interface IJobStateMachineRepository
{
    Result Store(JobStateMachine jobStateMachine);
    Option<JobStateMachine> Get(JobId jobId);
    Result Remove(JobId jobId);
}
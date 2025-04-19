using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Interfaces;

/// <summary>
///     Interface for creating job containers.
/// </summary>
public interface IJobWorkflowFactory
{
    /// <summary>
    ///     Asynchronously creates a job container for the specified job.
    /// </summary>
    /// <param name="job">The job for which to create the container.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a
    ///     <see cref="Result" />.
    /// </returns>
    Task<Result<JobStateMachine>> CreateAsync(Job job);
}
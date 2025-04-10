using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

/// <summary>
///     Interface for creating job containers.
/// </summary>
public interface IJobContainerFactory
{
    /// <summary>
    ///     Asynchronously creates a job container for the specified job.
    /// </summary>
    /// <param name="job">The job for which to create the container.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a
    ///     <see cref="Result" />.
    /// </returns>
    Task<Result<JobContainer>> CreateAsync(Job job);
}
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Batches;
using MediaBedrock.Cli.Domain.Jobs.Parameters;

namespace MediaBedrock.Cli.Domain.Jobs.Interfaces;

/// <summary>
///     Interface for creating jobs from templates and batch jobs.
/// </summary>
public interface IJobFactory
{
    /// <summary>
    ///     Asynchronously creates a job from the specified parameters.
    /// </summary>
    /// <param name="parameters">The parameters to use for the job.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a result with the created job or
    ///     an error.
    /// </returns>
    Task<Result<Job>> CreateAsync(JobParameters parameters);

    /// <summary>
    ///     Asynchronously creates a batch job from the specified parameters.
    /// </summary>
    /// <param name="parameters">The parameters to use for the batch job.</param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a result with the created batch
    ///     job or an error.
    /// </returns>
    Task<Result<BatchJob>> CreateAsync(BatchJobParameters parameters);
}
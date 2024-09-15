using Coderynx.Functional.Result;
using MediaBedrock.Cli.Domain.Jobs;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

/// <summary>
///     Interface for running jobs.
/// </summary>
public interface IJobRunner
{
    /// <summary>
    ///     Executes a single job asynchronously.
    /// </summary>
    /// <param name="job">The job to execute.</param>
    /// <param name="ct">Optional. A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result" /> object.</returns>
    Task<Result> TakeAsync(Job job, CancellationToken ct = default);

    /// <summary>
    ///     Executes a list of jobs asynchronously.
    /// </summary>
    /// <param name="jobs">The list of jobs to execute.</param>
    /// <param name="ct">Optional. A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result" /> object.</returns>
    Task<Result> TakeAsync(List<Job> jobs, CancellationToken ct = default);
}
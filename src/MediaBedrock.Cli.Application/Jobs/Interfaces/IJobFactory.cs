using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Batches;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.Jobs.Templates;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

/// <summary>
///     Interface for creating jobs from templates and batch jobs.
/// </summary>
public interface IJobFactory
{
    /// <summary>
    ///     Creates a job from the specified template and parameters.
    /// </summary>
    /// <param name="template">The job template to use for creating the job.</param>
    /// <param name="parameters">The parameters to use for the job.</param>
    /// <returns>A result containing the created job or an error.</returns>
    Result<Job> Create(JobTemplate template, JobParameters parameters);

    /// <summary>
    ///     Creates multiple jobs from the specified template and parameters.
    /// </summary>
    /// <param name="template">The job template to use for creating the jobs.</param>
    /// <param name="parameters">The parameters to use for the jobs.</param>
    /// <returns>A result containing the created jobs or an error.</returns>
    Result<BatchJob> Create(JobTemplate template, JobParameters[] parameters);
}
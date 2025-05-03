using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.BatchJobs;
using MediaBedrock.Cli.Domain.Jobs.Parameters;
using MediaBedrock.Cli.Domain.JobTemplates;

namespace MediaBedrock.Cli.Domain.Jobs.Interfaces;

/// <summary>
///     Interface for creating jobs from templates and batch jobs.
/// </summary>
public interface IJobFactory
{
    /// <summary>
    ///     Creates a single job based on the provided job template and parameters.
    /// </summary>
    /// <param name="template">The template defining the structure and configuration of the job.</param>
    /// <param name="parameters">The parameters to customize the job creation process.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the created <see cref="Job" /> if successful,
    ///     or an error result if the creation fails.
    /// </returns>
    Result<Job> Create(JobTemplate template, JobParameters parameters);

    /// <summary>
    ///     Creates a batch job consisting of multiple jobs based on the provided templates and parameters.
    /// </summary>
    /// <param name="templates">A list of templates defining the structure and configuration of each job in the batch.</param>
    /// <param name="parameters">The parameters to customize the batch job creation process.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the created <see cref="BatchJob" /> if successful,
    ///     or an error result if the creation fails.
    /// </returns>
    Result<BatchJob> Create(List<JobTemplate> templates, BatchJobParameters parameters);
}
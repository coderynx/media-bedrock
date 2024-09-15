using Coderynx.Functional.Result;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Batch;
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
    /// <param name="inputs">The input parameters for the job.</param>
    /// <param name="outputs">The output parameters for the job.</param>
    /// <param name="properties">The property parameters for the job.</param>
    /// <returns>A result containing the created job or an error.</returns>
    Result<Job> Create(JobTemplate template,
        JobInputParameter[] inputs,
        JobOutputParameter[] outputs,
        JobPropertyParameter[] properties);

    /// <summary>
    ///     Creates multiple jobs from the specified template and batch job.
    /// </summary>
    /// <param name="template">The job template to use for creating the jobs.</param>
    /// <param name="batchJob">The batch job containing multiple job entries.</param>
    /// <returns>A result containing the created jobs or an error.</returns>
    Result<IEnumerable<Job>> Create(JobTemplate template, BatchJob batchJob);
}
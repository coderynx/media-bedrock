using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Assets;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Cli.Domain.Jobs.Interfaces;

/// <summary>
///     Factory interface for creating instances of <see cref="ProcessorContext" />.
/// </summary>
public interface IProcessorContextFactory
{
    /// <summary>
    ///     Creates a new <see cref="ProcessorContext" /> for the specified processor type, job, step, and assets pool.
    /// </summary>
    /// <param name="processorType">The type of the processor for which the context is being created.</param>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="step">The job step associated with the processor context.</param>
    /// <param name="assetsPool">The pool of assets available for the job.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the created <see cref="ProcessorContext" /> if successful,
    ///     or an error result if the context creation fails.
    /// </returns>
    Result<ProcessorContext> Create(Type processorType, JobId jobId, JobStep step, JobAssetsPool assetsPool);
}
using Coderynx.Functional.Results;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Cli.Domain.Processors.Interfaces;

/// <summary>
///     Provides methods to resolve processors and their configurations by name.
/// </summary>
public interface IProcessorProvider
{
    /// <summary>
    ///     Resolves a processor instance based on the given processor name.
    /// </summary>
    /// <param name="name">The name of the processor to resolve.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the resolved <see cref="IProcessor" /> if successful,
    ///     or an error result if the processor cannot be resolved.
    /// </returns>
    Result<IProcessor> ResolveProcessor(ProcessorName name);

    /// <summary>
    ///     Resolves the configuration for a processor based on the given processor name.
    /// </summary>
    /// <param name="name">The name of the processor whose configuration is to be resolved.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the resolved <see cref="ProcessorConfiguration" /> if successful,
    ///     or an error result if the configuration cannot be resolved.
    /// </returns>
    Result<ProcessorConfiguration> ResolveConfiguration(ProcessorName name);
}
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Batches;

namespace MediaBedrock.Cli.Domain.Jobs.Interfaces;

/// <summary>
///     Defines methods for serializing and deserializing batch job parameters.
/// </summary>
public interface IBatchJobParametersSerializer
{
    /// <summary>
    ///     Serializes the given <see cref="BatchJobParameters" /> into a string representation.
    /// </summary>
    /// <param name="parameters">The batch job parameters to serialize.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the serialized string if successful,
    ///     or an error result if the serialization fails.
    /// </returns>
    Result<string> Serialize(BatchJobParameters parameters);

    /// <summary>
    ///     Deserializes the given string into a <see cref="BatchJobParameters" /> object.
    /// </summary>
    /// <param name="serialized">The string representation of batch job parameters.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the deserialized <see cref="BatchJobParameters" />
    ///     if successful, or an error result if the deserialization fails.
    /// </returns>
    Result<BatchJobParameters> Deserialize(string serialized);
}
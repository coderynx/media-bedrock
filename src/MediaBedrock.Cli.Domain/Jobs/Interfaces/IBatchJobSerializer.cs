using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Batches;

namespace MediaBedrock.Cli.Domain.Jobs.Interfaces;

/// <summary>
///     Defines methods for serializing and deserializing batch jobs.
/// </summary>
public interface IBatchJobSerializer
{
    /// <summary>
    ///     Serializes the given <see cref="BatchJob" /> into a string representation.
    /// </summary>
    /// <param name="batchJob">The batch job to serialize.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the serialized string if successful,
    ///     or an error result if the serialization fails.
    /// </returns>
    Result<string> Serialize(BatchJob batchJob);

    /// <summary>
    ///     Deserializes the given string into a <see cref="BatchJob" /> object.
    /// </summary>
    /// <param name="serialized">The string representation of a batch job.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the deserialized <see cref="BatchJob" />
    ///     if successful, or an error result if the deserialization fails.
    /// </returns>
    Result<BatchJob> Deserialize(string serialized);
}
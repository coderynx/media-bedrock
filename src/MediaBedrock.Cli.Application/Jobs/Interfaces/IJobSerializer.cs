using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

/// <summary>
///     Interface for serializing and deserializing job objects.
/// </summary>
public interface IJobSerializer
{
    /// <summary>
    ///     Serializes a job object into a string representation.
    /// </summary>
    /// <param name="job">The job object to serialize.</param>
    /// <returns>A result containing the serialized string or an error.</returns>
    Result<string> Serialize(Job job);

    /// <summary>
    ///     Deserializes a string representation of a job into a job object.
    /// </summary>
    /// <param name="serialized">The serialized string representation of the job.</param>
    /// <returns>A result containing the deserialized job object or an error.</returns>
    Result<Job> Deserialize(string serialized);
}
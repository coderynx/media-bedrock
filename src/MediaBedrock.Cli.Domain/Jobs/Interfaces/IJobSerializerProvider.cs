using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Interfaces;

/// <summary>
///     Provides methods to resolve job serializers based on format or file path.
/// </summary>
public interface IJobSerializerProvider
{
    /// <summary>
    ///     Resolves a job serializer based on the specified format.
    /// </summary>
    /// <param name="format">The format of the job serializer to resolve.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the resolved <see cref="IJobSerializer" />
    ///     if successful, or an error result if the resolution fails.
    /// </returns>
    Result<IJobSerializer> ResolveSerializer(JobSerializerFormat format);

    /// <summary>
    ///     Resolves a job serializer based on the specified file path.
    /// </summary>
    /// <param name="path">The file path to resolve the job serializer from.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the resolved <see cref="IJobSerializer" />
    ///     if successful, or an error result if the resolution fails.
    /// </returns>
    Result<IJobSerializer> ResolveSerializer(string path);
}
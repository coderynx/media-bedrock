using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Templates.Interfaces;

/// <summary>
///     Provides functionality to resolve an appropriate <see cref="IJobTemplateSerializer" />
///     based on the specified format or file path.
/// </summary>
public interface IJobTemplateSerializerProvider
{
    /// <summary>
    ///     Resolves a serializer for the specified <see cref="JobTemplateSerializerFormat" />.
    /// </summary>
    /// <param name="format">The format of the job template serializer to resolve.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the resolved <see cref="IJobTemplateSerializer" />
    ///     if successful, or an error result if resolution fails.
    /// </returns>
    Result<IJobTemplateSerializer> ResolveSerializer(JobTemplateSerializerFormat format);

    /// <summary>
    ///     Resolves a serializer based on the specified file path.
    /// </summary>
    /// <param name="extension">The file extension to resolve the serializer for.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> containing the resolved <see cref="IJobTemplateSerializer" />
    ///     if successful, or an error result if resolution fails.
    /// </returns>
    Result<IJobTemplateSerializer> ResolveSerializer(string extension);
}
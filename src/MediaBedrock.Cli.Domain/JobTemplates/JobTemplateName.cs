using Coderynx.Functional.Results;
using Coderynx.Functional.Results.Errors;

namespace MediaBedrock.Cli.Domain.JobTemplates;

/// <summary>
///     Represents the name of a job template.
/// </summary>
public sealed record JobTemplateName
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JobTemplateName" /> class.
    /// </summary>
    /// <param name="value">The name of the job template.</param>
    /// <exception cref="ErrorException">Thrown when the name is invalid.</exception>
    public JobTemplateName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw JobTemplateErrors.InvalidName(value);
        }

        Value = value;
    }

    /// <summary>
    ///     Gets the name of the job template.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value;
    }
    
    /// <summary>
    ///     Creates a new instance of <see cref="JobTemplateName" />.
    /// </summary>
    /// <param name="name">The name of the job template.</param>
    /// <returns>A <see cref="Result{JobTemplateName}" /> representing the success or failure of the operation.</returns>
    public static Result<JobTemplateName> Create(string name)
    {
        return Result.TryCatch(() => new JobTemplateName(name));
    }
}
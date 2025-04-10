using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Templates;

/// <summary>
///     Represents the name of a job template.
/// </summary>
public sealed record JobTemplateName
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="JobTemplateName" /> class.
    /// </summary>
    /// <param name="name">The name of the job template.</param>
    private JobTemplateName(string name)
    {
        Value = name;
    }

    /// <summary>
    ///     Gets the value of the job template name.
    /// </summary>
    public string Value { get; init; }

    /// <summary>
    ///     Creates a new <see cref="JobTemplateName" /> instance.
    /// </summary>
    /// <param name="name">The name of the job template.</param>
    /// <returns>
    ///     A <see cref="Result" /> containing the <see cref="JobTemplateName" /> instance if the name is valid;
    ///     otherwise, an error result indicating the name is invalid.
    /// </returns>
    public static Result<JobTemplateName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return JobTemplateErrors.InvalidName(name);
        }

        var jobTemplateName = new JobTemplateName(name);
        return Result.Created(jobTemplateName);
    }
}
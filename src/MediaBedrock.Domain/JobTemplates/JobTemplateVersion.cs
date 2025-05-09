using System.Text.RegularExpressions;
using Coderynx.Functional.Results;

namespace MediaBedrock.Domain.JobTemplates;

public sealed partial record JobTemplateVersion
{
    public JobTemplateVersion(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !SemverRegex().IsMatch(value))
        {
            throw JobTemplateErrors.InvalidVersion(value);
        }

        Value = value;
    }

    public JobTemplateVersion()
    {
        Value = "0.0.1";
    }

    public string Value { get; }

    public static Result<JobTemplateVersion> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !SemverRegex().IsMatch(value))
        {
            return JobTemplateErrors.InvalidVersion(value);
        }

        var version = new JobTemplateVersion(value);

        return Result.Created(version);
    }

    public override string ToString()
    {
        return Value;
    }

    [GeneratedRegex(
        @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?(?:\+([0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*))?$")]
    private static partial Regex SemverRegex();
}
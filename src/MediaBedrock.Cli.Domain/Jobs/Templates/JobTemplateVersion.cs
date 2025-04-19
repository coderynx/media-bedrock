using System.Text.RegularExpressions;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Templates;

public sealed partial record JobTemplateVersion
{
    public static readonly JobTemplateVersion Default = new()
    {
        Value = "0.0.1"
    };

    private JobTemplateVersion()
    {
    }

    public required string Value { get; init; }

    public static Result<JobTemplateVersion> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return JobTemplateErrors.InvalidVersion(value);
        }

        var semverRegex = SemverRegex();
        if (!semverRegex.IsMatch(value))
        {
            return JobTemplateErrors.InvalidVersion(value);
        }

        var version = new JobTemplateVersion
        {
            Value = value
        };

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
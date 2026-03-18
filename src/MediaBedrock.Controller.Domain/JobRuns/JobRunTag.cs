using Coderynx.Functional.Results;

namespace MediaBedrock.Controller.Domain.JobRuns;

public sealed record JobRunTag
{
    public JobRunTag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw JobRunErrors.InvalidTag(value);
        }

        Value = value;
    }

    public string Value { get; }

    public static Result<JobRunTag> Create(string value)
    {
        return Result.TryCatch(() => new JobRunTag(value));
    }
}
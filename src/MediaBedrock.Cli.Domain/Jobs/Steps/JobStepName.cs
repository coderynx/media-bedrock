using Coderynx.Functional.Result;

namespace MediaBedrock.Cli.Domain.Jobs.Steps;

public sealed record JobStepName
{
    private JobStepName()
    {
    }

    public required string Value { get; init; }

    public static Result<JobStepName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return JobErrors.InvalidStepName(name);
        }

        var stepName = new JobStepName { Value = name };
        return Result.Created(stepName);
    }

    public override string ToString()
    {
        return Value;
    }
}
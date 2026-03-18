using Coderynx.Functional.Results;

namespace MediaBedrock.Controller.Domain.Jobs.Steps;

public sealed record JobStepName
{
    public JobStepName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw JobErrors.InvalidStepName(name);
        }

        Value = name;
    }

    public string Value { get; }

    public static Result<JobStepName> Create(string name)
    {
        return Result.TryCatch(() => new JobStepName(name));
    }

    public override string ToString()
    {
        return Value;
    }
}
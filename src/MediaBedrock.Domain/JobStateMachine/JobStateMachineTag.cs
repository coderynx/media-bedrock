using Coderynx.Functional.Results;

namespace MediaBedrock.Domain.JobStateMachine;

public sealed record JobStateMachineTag
{
    public JobStateMachineTag(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw JobStateMachineErrors.InvalidTag(value);
        }

        Value = value;
    }

    public string Value { get; }

    public static Result<JobStateMachineTag> Create(string value)
    {
        return Result.TryCatch(() => new JobStateMachineTag(value));
    }
}
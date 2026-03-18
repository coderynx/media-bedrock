using Coderynx.Functional.Results;

namespace MediaBedrock.Controller.Domain.Jobs.Steps;

public sealed record JobStepOrder
{
    public JobStepOrder(uint value)
    {
        if (value is 0)
        {
            throw JobErrors.InvalidStepOrder(value);
        }

        Value = value;
    }

    public uint Value { get; init; }

    public static Result<JobStepOrder> Create(uint value)
    {
        return Result.TryCatch(() => new JobStepOrder(value));
    }
}
using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.Jobs.Steps;

namespace MediaBedrock.Controller.Domain.JobRuns;

public sealed record JobRunStepOrder
{
    public JobRunStepOrder(uint value)
    {
        if (value is 0)
        {
            throw JobRunErrors.InvalidStepOrder(value);
        }

        Value = value;
    }

    public uint Value { get; init; }

    public static Result<JobRunStepOrder> Create(uint value)
    {
        return Result.TryCatch(() => new JobRunStepOrder(value));
    }

    public static JobRunStepOrder Create(JobStepOrder jobStepOrder)
    {
        return new JobRunStepOrder(jobStepOrder.Value);
    }
}
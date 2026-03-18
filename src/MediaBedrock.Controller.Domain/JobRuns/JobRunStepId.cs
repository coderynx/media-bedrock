using Coderynx.Functional.Results;

namespace MediaBedrock.Controller.Domain.JobRuns;

public sealed record JobRunStepId
{
    public JobRunStepId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw JobRunErrors.InvalidStepId();
        }

        Value = value;
    }

    public JobRunStepId()
    {
        Value = Guid.CreateVersion7();
    }

    public Guid Value { get; }

    public static Result<JobRunStepId> Create(Guid value)
    {
        return Result.TryCatch(() => new JobRunStepId(value));
    }
}
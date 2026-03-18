using Coderynx.Functional.Results;

namespace MediaBedrock.Worker.Domain.Processing.ValueObjects;

public sealed record JobRunStepId
{
    public JobRunStepId(Guid value)
    {
        if (Value == Guid.Empty)
        {
            throw ProcessorInstanceErrors.InvalidJobRunStepId(value);
        }

        Value = value;
    }

    public Guid Value { get; init; } = Guid.NewGuid();
    
    public static Result<JobRunStepId> Create(Guid value)
    {
        return Result.TryCatch(() => new JobRunStepId(value));
    }
    
    public override string ToString()
    {
        return Value.ToString();
    }
}
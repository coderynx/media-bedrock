using Coderynx.Functional.Results;

namespace MediaBedrock.Controller.Domain.JobTemplates;

public sealed record JobTemplateStepOrder
{
    public JobTemplateStepOrder(uint value)
    {
        if (value is 0)
        {
            throw JobTemplateErrors.InvalidStepOrder(value);
        }

        Value = value;
    }

    public uint Value { get; init; }

    public static Result<JobTemplateStepOrder> Create(uint value)
    {
        return Result.TryCatch(() => new JobTemplateStepOrder(value));
    }
}
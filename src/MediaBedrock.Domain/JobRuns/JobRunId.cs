using Coderynx.Functional.Results;

namespace MediaBedrock.Domain.JobRuns;

public sealed record JobRunId
{
    public JobRunId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw JobRunErrors.InvalidId();
        }

        Value = value;
    }

    public Guid Value { get; }

    public static JobRunId Create()
    {
        return new JobRunId(Guid.CreateVersion7());
    }

    public static Result<JobRunId> Create(Guid value)
    {
        return Result.TryCatch(() => new JobRunId(value));
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
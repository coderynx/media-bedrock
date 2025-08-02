namespace MediaBedrock.Domain.JobRuns;

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
}
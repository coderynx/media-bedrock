namespace MediaBedrock.Domain.JobStateMachine;

public sealed record JobStateMachineId
{
    public JobStateMachineId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw JobStateMachineErrors.InvalidId();
        }

        Value = value;
    }

    public Guid Value { get; }

    public static JobStateMachineId Create()
    {
        return new JobStateMachineId(Guid.CreateVersion7());
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
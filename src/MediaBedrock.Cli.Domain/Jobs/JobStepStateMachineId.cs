namespace MediaBedrock.Cli.Domain.Jobs;

public sealed record JobStepStateMachineId
{
    public JobStepStateMachineId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException(JobStateMachineErrors.InvalidStepIdCode, "Step ID cannot be empty.");
        }

        Value = value;
    }

    public JobStepStateMachineId()
    {
        Value = Guid.CreateVersion7();
    }

    public Guid Value { get; init; }
}
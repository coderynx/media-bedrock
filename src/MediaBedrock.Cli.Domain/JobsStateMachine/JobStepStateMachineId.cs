
namespace MediaBedrock.Cli.Domain.JobsStateMachine;

public sealed record JobStepStateMachineId
{
    public JobStepStateMachineId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw JobStateMachineErrors.InvalidStepId();
        }

        Value = value;
    }

    public JobStepStateMachineId()
    {
        Value = Guid.CreateVersion7();
    }

    public Guid Value { get; }
}
namespace MediaBedrock.Cli.Domain.JobsStateMachine;

public sealed record JobStateMachineId(Guid Value)
{
    public static JobStateMachineId Create()
    {
        return new JobStateMachineId(Guid.CreateVersion7());
    }
}
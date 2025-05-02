namespace MediaBedrock.Cli.Domain.Jobs;

public sealed record JobStateMachineId(Guid Value)
{
    public static JobStateMachineId Create()
    {
        return new JobStateMachineId(Guid.CreateVersion7());
    }
}
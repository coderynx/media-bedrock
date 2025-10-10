namespace MediaBedrock.Domain.Jobs.Steps;

public sealed record JobStepId(Guid Value)
{
    public static JobStepId Create()
    {
        return new JobStepId(Guid.NewGuid());
    }
}
namespace MediaBedrock.Worker.Domain.Processing;

public enum ProcessorInstanceStatus
{
    Pending,
    Running,
    Completed,
    Failed
}
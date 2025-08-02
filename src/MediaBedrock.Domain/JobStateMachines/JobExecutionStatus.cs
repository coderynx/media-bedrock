namespace MediaBedrock.Domain.JobStateMachines;

public enum JobExecutionStatus
{
    Pending,
    Running,
    Completed,
    Failed
}
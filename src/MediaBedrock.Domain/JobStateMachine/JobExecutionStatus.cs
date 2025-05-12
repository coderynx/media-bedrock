namespace MediaBedrock.Domain.JobStateMachine;

public enum JobExecutionStatus
{
    Pending,
    Running,
    Completed,
    Failed
}
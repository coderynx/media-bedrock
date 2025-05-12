namespace MediaBedrock.Domain.JobStateMachine;

public enum JobStepExecutionStatus
{
    Pending,
    Running,
    Completed,
    Failed
}
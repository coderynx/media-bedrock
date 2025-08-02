namespace MediaBedrock.Domain.JobStateMachines;

public enum JobStepExecutionStatus
{
    Pending,
    Running,
    Completed,
    Failed
}
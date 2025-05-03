namespace MediaBedrock.Cli.Domain.JobsStateMachine;

public enum JobStepExecutionStatus
{
    Pending,
    Running,
    Completed,
    Failed
}
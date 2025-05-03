namespace MediaBedrock.Cli.Domain.JobsStateMachine;

public enum JobExecutionStatus
{
    Pending,
    Running,
    Completed,
    Failed
}
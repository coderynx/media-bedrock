namespace MediaBedrock.Domain.JobStateMachine;

public sealed record JobExecutionError
{
    public JobExecutionError(JobFailureReason jobFailureReason, string message = "")
    {
        FailureReason = jobFailureReason;
        Message = message;
    }

    public JobFailureReason FailureReason { get; }
    public string Message { get; }
}
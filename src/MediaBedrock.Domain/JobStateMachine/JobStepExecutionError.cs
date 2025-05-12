namespace MediaBedrock.Domain.JobStateMachine;

public sealed record JobStepExecutionError
{
    public JobStepExecutionError(JobStepFailureReason reason, string message = "")
    {
        Reason = reason;
        Message = message;
    }

    public JobStepFailureReason Reason { get; }
    public string Message { get; }
}
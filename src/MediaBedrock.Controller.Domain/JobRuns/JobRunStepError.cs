namespace MediaBedrock.Controller.Domain.JobRuns;

public sealed record JobRunStepError
{
    public JobRunStepError(JobStepFailureReason reason, string message = "")
    {
        Reason = reason;
        Message = message;
    }

    public JobStepFailureReason Reason { get; }
    public string Message { get; }
}
namespace MediaBedrock.Cli.Domain.JobsStateMachine;

public sealed record JobStepExecutionError
{
    public required JobStepFailureReason Kind { get; init; }
    public string Message { get; init; } = string.Empty;

    public static JobStepExecutionError Create(JobStepFailureReason failureReason, string message)
    {
        return new JobStepExecutionError
        {
            Kind = failureReason,
            Message = message
        };
    }
}
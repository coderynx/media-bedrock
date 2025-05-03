namespace MediaBedrock.Cli.Domain.JobsStateMachine;

public enum JobStepFailureReason
{
    None = 0,
    InputValidation = 1,
    Processing = 2,
    OutputAssetsAssessment = 3,
    Unknown = 4
}
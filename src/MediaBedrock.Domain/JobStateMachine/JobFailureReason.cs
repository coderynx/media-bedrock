namespace MediaBedrock.Domain.JobStateMachine;

public enum JobFailureReason
{
    InputValidation = 0,
    Processing = 1,
    OutputAssetsAssessment = 2,
    Unknown = 3
}
namespace MediaBedrock.Domain.JobRuns;

public enum JobStepFailureReason
{
    InputValidation = 0,
    Processing = 1,
    OutputAssetsAssessment = 2,
    Unknown = 3
}
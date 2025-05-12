using Coderynx.Functional.Results.Errors;
using MediaBedrock.Domain.Jobs.Steps;
using MediaBedrock.Domain.JobStateMachine;

namespace MediaBedrock.Domain.Jobs;

public static class JobErrorCodes
{
    public const string PropertyNotFound = "Job.PropertyNotFound";
    public const string InvalidStepName = "Job.InvalidStepName";
    public const string InvalidInputName = "Job.InvalidInputName";
    public const string InvalidOutputName = "Job.InvalidOutputName";
    public const string StepNotFound = "Job.StepNotFound";
    public const string NotFound = "Job.NotFound";
    public const string StateMachineNotFound = "Job.StateMachineNotFound";
    public const string InvalidId = "Job.InvalidId";
    public const string StoreFailed = "Job.StoreFailed";
}

public static class JobErrors
{
    public static Error PropertyNotFound(string key)
    {
        return Error.InvalidInput(
            code: JobErrorCodes.PropertyNotFound,
            message: $"The job property '{key}' was not found. Please check the property name and try again.");
    }

    public static Error InvalidStepName(string name)
    {
        return Error.InvalidInput(
            code: JobErrorCodes.InvalidStepName,
            message: $"The job step name '{name}' is invalid. Please check the name and try again.");
    }

    public static Error SerializationFailed(string message)
    {
        return Error.InvalidInput(
            code: "Job.SerializationFailed",
            message: $"The job serialization failed with error: {message}");
    }

    public static Error InvalidInputName(string name)
    {
        return Error.InvalidInput(
            code: JobErrorCodes.InvalidInputName,
            message: $"The job step input name '{name}' is invalid. Please check the name and try again.");
    }

    public static Error InvalidOutputName(string name)
    {
        return Error.InvalidInput(
            code: JobErrorCodes.InvalidOutputName,
            message: $"The job step output '{name}' is invalid. Please check the name and try again.");
    }

    public static Error StepNotFound(JobStepName name)
    {
        return Error.InvalidInput(
            code: JobErrorCodes.StepNotFound,
            message: $"The job step '{name}' was not found. Please check the step name and try again.");
    }

    public static Error NotFound(JobId jobId)
    {
        return Error.NotFound(
            code: JobErrorCodes.NotFound,
            message: $"The job with ID '{jobId}' was not found.");
    }

    public static Error StateMachineNotFound(JobStateMachineId stateMachineId)
    {
        return Error.NotFound(
            code: JobErrorCodes.StateMachineNotFound,
            message: $"The job state machine with ID '{stateMachineId}' was not found.");
    }

    public static Error InvalidId()
    {
        return Error.InvalidInput(
            code: JobErrorCodes.InvalidId,
            message: "The job ID is invalid. Please check the ID and try again.");
    }

    public static Error StoreFailed(JobId valueId)
    {
        return Error.Custom(
            code: JobErrorCodes.StoreFailed,
            message: $"The job with ID '{valueId}' could not be stored. Please check the job and try again.");
    }
}
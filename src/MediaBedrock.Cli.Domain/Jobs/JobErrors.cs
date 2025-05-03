using Coderynx.Functional;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.JobsStateMachine;

namespace MediaBedrock.Cli.Domain.Jobs;

public static class JobErrors
{
    public const string PropertyNotFoundCode = "Job.PropertyNotFound";

    public const string InvalidStepNameCode = "Job.InvalidStepName";

    public const string DeserializationFailedCode = "Job.DeserializationFailed";

    public const string InvalidInputNameCode = "Job.InvalidInputName";

    public const string InvalidOutputNameCode = "Job.InvalidOutputName";

    public const string StepNotFoundCode = "Job.StepNotFound";

    public const string NotFoundCode = "Job.NotFound";

    public const string StateMachineNotFoundCode = "Job.StateMachineNotFound";

    public static Error PropertyNotFound(string key)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: PropertyNotFoundCode,
            Message: $"The job property '{key}' was not found. Please check the property name and try again.");
    }

    public static Error InvalidStepName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidStepNameCode,
            Message: $"The job step name '{name}' is invalid. Please check the name and try again.");
    }

    public static Error SerializationFailed(string message)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Job.SerializationFailed",
            Message: $"The job serialization failed with error: {message}");
    }

    public static Error DeserializationFailed(string serialized)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: DeserializationFailedCode,
            Message: $"The job deserialization failed for the serialized job: {serialized}");
    }

    public static Error InvalidInputName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidInputNameCode,
            Message: $"The job step input name '{name}' is invalid. Please check the name and try again.");
    }

    public static Error InvalidOutputName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidOutputNameCode,
            Message: $"The job step output '{name}' is invalid. Please check the name and try again.");
    }

    public static Error StepNotFound(JobStepName name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: StepNotFoundCode,
            Message: $"The job step '{name}' was not found. Please check the step name and try again.");
    }

    public static Error NotFound(JobId jobId)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: NotFoundCode,
            Message: $"The job with ID '{jobId}' was not found.");
    }

    public static Error StateMachineNotFound(JobStateMachineId stateMachineId)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: StateMachineNotFoundCode,
            Message: $"The job state machine with ID '{stateMachineId}' was not found.");
    }
}
using Coderynx.Functional;
using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Steps;

namespace MediaBedrock.Cli.Domain.Jobs;

public static class JobErrors
{
    public const string PropertyNotFoundCode = "Job.PropertyNotFound";

    public const string InvalidIdCode = "Job.InvalidId";

    public const string InvalidStepNameCode = "Job.InvalidStepName";

    public const string DeserializationFailedCode = "Job.DeserializationFailed";

    public const string ContainerConflictCode = "Job.ContainerConflict";

    public const string ContainerRemovalFailedCode = "Job.ContainerRemovalFailed";

    public const string InvalidAssetNameCode = "Job.InvalidAssetName";

    public const string InvalidSinkNameCode = "Job.InvalidSinkName";

    public const string InvalidSourceNameCode = "Job.InvalidSourceName";

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

    public static Error InvalidId(Guid id)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidIdCode,
            Message: $"The job ID '{id}' is invalid. Please check the ID and try again.");
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

    public static Error ContainerConflict(JobId jobId)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: ContainerConflictCode,
            Message: $"A job container with ID '{jobId}' already exists.");
    }

    public static Error ContainerRemovalFailed(JobId jobId)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: ContainerRemovalFailedCode,
            Message: $"Failed to remove the job container with ID '{jobId}'.");
    }

    public static Error InvalidAssetName(string assetName)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidAssetNameCode,
            Message: $"The job asset name '{assetName}' is invalid. Please check the name and try again.");
    }

    public static Error InvalidSinkName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidSinkNameCode,
            Message: $"The job step sink name '{name}' is invalid. Please check the name and try again.");
    }

    public static Error InvalidSourceName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidSourceNameCode,
            Message: $"The job step source name '{name}' is invalid. Please check the name and try again.");
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
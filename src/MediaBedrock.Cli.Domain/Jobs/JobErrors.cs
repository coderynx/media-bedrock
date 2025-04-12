using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs;

public static class JobErrors
{
    public const string PropertyNotFoundCode = "Job.PropertyNotFound";

    public const string InvalidIdCode = "Job.InvalidId";

    public const string InvalidStepNameCode = "Job.InvalidStepName";

    public const string DeserializationFailedCode = "Job.DeserializationFailed";

    public const string ContainerConflictCode = "Job.ContainerConflict";

    public const string ContainerRemovalFailedCode = "Job.ContainerRemovalFailed";

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
            Message: $"A job container with ID '{jobId}' already exists. Please check the job ID and try again.");
    }

    public static Error ContainerRemovalFailed(JobId jobId)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: ContainerRemovalFailedCode,
            Message: $"Failed to remove the job container with ID '{jobId}'. Please check the job ID and try again.");
    }
}
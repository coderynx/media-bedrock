using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs;

public static class JobErrors
{
    public static Error PropertyNotFound(string key)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Job.PropertyNotFound",
            Message: $"The job property '{key}' was not found. Please check the property name and try again.");
    }

    public static Error InvalidId(Guid id)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Job.InvalidId",
            Message: $"The job ID '{id}' is invalid. Please check the ID and try again.");
    }

    public static Error InvalidStepName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Job.InvalidJobStepName",
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
            Code: "Job.DeserializationFailed",
            Message: $"The job deserialization failed for the serialized job: {serialized}");
    }
}
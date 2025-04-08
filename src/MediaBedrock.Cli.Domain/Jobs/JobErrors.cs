using Coderynx.Functional;
using Coderynx.Functional.Result;

namespace MediaBedrock.Cli.Domain.Jobs;

public static class JobErrors
{
    public static Error InputParameterNotFound(string inputName)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Job.InputParameterNotFound",
            Message:
            $"The job input parameter '{inputName}' was not found. Please check the input name and try again.");
    }

    public static Error OutputParameterNotFound(string outputName)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Job.OutputParameterNotFound",
            Message:
            $"The job output parameter '{outputName}' was not found. Please check the output name and try again.");
    }

    public static Error InvalidInputString(string input)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Job.InvalidInputString",
            Message: $"The job input string'{input}' is invalid. Please check the format and try again.");
    }

    public static Error InvalidOutputString(string outputString)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Job.InvalidOutputString",
            Message: $"The job output string '{outputString}' is invalid. Please check the format and try again.");
    }

    public static Error InvalidPropertyString(string propertyString)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Job.InvalidPropertyString",
            Message: $"The job property string '{propertyString}' is invalid. Please check the format and try again.");
    }

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
using Coderynx.Functional;
using Coderynx.Functional.Result;

namespace MediaBedrock.Cli.Domain.Jobs.Templates;

public static class JobTemplateErrors
{
    public static Error NotFound(string path)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: "JobTemplate.NotFound",
            Message: $"The job template file '{path}' was not found. Please check the path and try again.");
    }

    public static Error DeserializationFailed(string path)
    {
        return new Error(
            ResultError: ResultError.Custom,
            Code: "JobTemplate.DeserializationFailed",
            Message:
            $"Failed to deserialize the job template file '{path}'. Please check the file format and try again.");
    }

    public static Error InvalidName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "JobTemplate.InvalidName",
            Message: $"The job template name '{name}' is invalid. Please check the format and try again.");
    }

    public static Error SerializationFailed(string message)
    {
        return new Error(
            ResultError: ResultError.Custom,
            Code: "JobTemplate.SerializationFailed",
            Message: $"Failed to serialize the job template. {message}");
    }
}
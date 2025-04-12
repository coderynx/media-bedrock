using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Templates;

public static class JobTemplateErrors
{
    public const string NotFoundCode = "JobTemplate.NotFound";

    public const string DeserializationFailedCode = "JobTemplate.DeserializationFailed";

    public const string InvalidNameCode = "JobTemplate.InvalidName";

    public const string SerializationFailedCode = "JobTemplate.SerializationFailed";

    public const string StoreFailedCode = "JobTemplate.StoreFailed";

    public const string DeleteFailedCode = "JobTemplate.DeleteFailed";

    public static Error NotFound(JobTemplateName name)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: NotFoundCode,
            Message: $"The job template '{name}' was not found. Please check the name and try again.");
    }

    public static Error DeserializationFailed(string path)
    {
        return new Error(
            ResultError: ResultError.Custom,
            Code: DeserializationFailedCode,
            Message:
            $"Failed to deserialize the job template file '{path}'. Please check the file format and try again.");
    }

    public static Error InvalidName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidNameCode,
            Message: $"The job template name '{name}' is invalid. Please check the format and try again.");
    }

    public static Error SerializationFailed(string message)
    {
        return new Error(
            ResultError: ResultError.Custom,
            Code: SerializationFailedCode,
            Message: $"Failed to serialize the job template. {message}");
    }

    public static Error StoreFailed(string message)
    {
        return new Error(
            ResultError: ResultError.Custom,
            Code: StoreFailedCode,
            Message: $"Failed to store the job template. {message}");
    }

    public static Error DeleteFailed(string message)
    {
        return new Error(
            ResultError: ResultError.Custom,
            Code: DeleteFailedCode,
            Message: $"Failed to delete the job template. {message}");
    }
}
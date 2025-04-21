using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.JobTemplates;

public static class JobTemplateErrors
{
    public const string NotFoundCode = "JobTemplate.NotFound";

    public const string DeserializationFailedCode = "JobTemplate.DeserializationFailed";

    public const string InvalidNameCode = "JobTemplate.InvalidName";

    public const string SerializationFailedCode = "JobTemplate.SerializationFailed";

    public const string InvalidAuthorCode = "JobTemplate.InvalidAuthor";

    public const string InvalidVersionCode = "JobTemplate.InvalidVersion";

    public static Error ManifestNotFound(string path)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: NotFoundCode,
            Message: $"The job template manifest file '{path}' was not found.");
    }

    public static Error NotFound(JobTemplateName name)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: NotFoundCode,
            Message: $"The job template '{name}' was not found.");
    }

    public static Error DeserializationFailed(string path)
    {
        return new Error(
            ResultError: ResultError.Custom,
            Code: DeserializationFailedCode,
            Message:
            $"Failed to deserialize the job template file '{path}'.");
    }

    public static Error InvalidName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidNameCode,
            Message: $"The job template name '{name}' is invalid.");
    }

    public static Error SerializationFailed(string message)
    {
        return new Error(
            ResultError: ResultError.Custom,
            Code: SerializationFailedCode,
            Message: $"Failed to serialize the job template. {message}");
    }

    public static Error InvalidAuthor(string author)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidAuthorCode,
            Message: $"The job template author '{author}' is invalid.");
    }

    public static Error InvalidVersion(string version)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidVersionCode,
            Message: $"The job template version '{version}' is invalid.");
    }
}
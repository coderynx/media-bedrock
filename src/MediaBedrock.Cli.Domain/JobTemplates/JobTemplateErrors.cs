using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.JobTemplates;

public static class JobTemplateErrors
{
    public const string NotFoundCode = "JobTemplate.NotFound";
    public const string ManifestDeserializationFailedCode = "JobTemplate.ManifestDeserializationFailed";
    public const string InvalidNameCode = "JobTemplate.InvalidName";
    public const string ManifestSerializationFailedCode = "JobTemplate.ManifestSerializationFailed";
    public const string InvalidAuthorCode = "JobTemplate.InvalidAuthor";
    public const string InvalidVersionCode = "JobTemplate.InvalidVersion";
    public const string InvalidStepNameCode = "JobTemplate.InvalidStepName";
    public const string InvalidIdCode = "JobTemplate.InvalidId";
    public const string InvalidStepIdCode = "JobTemplate.InvalidStepId";
    public const string InvalidInputIdCode = "JobTemplate.InvalidInputId";
    public const string InvalidOutputIdCode = "JobTemplate.InvalidOutputId";
    public const string ConflictCode = "JobTemplate.Conflict";
    public const string StoreFailedCode = "JobTemplate.StoreFailed";

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

    public static Error ManifestDeserializationFailed(string path)
    {
        return new Error(
            ResultError: ResultError.Custom,
            Code: ManifestDeserializationFailedCode,
            Message:
            $"Failed to deserialize the job template manifest file '{path}'.");
    }

    public static Error InvalidName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidNameCode,
            Message: $"The job template name '{name}' is invalid.");
    }

    public static Error ManifestSerializationFailed(string message)
    {
        return new Error(
            ResultError: ResultError.Custom,
            Code: ManifestSerializationFailedCode,
            Message: $"Failed to serialize the job template manifest. {message}");
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

    public static Error InvalidStepName(string value)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: InvalidStepNameCode,
            Message: $"The job template step name '{value}' is invalid.");
    }

    public static Error Conflict(JobTemplateName name)
    {
        return new Error(
            ResultError: ResultError.Conflict,
            Code: ConflictCode,
            Message: $"The job template with ID '{name}' already exists.");
    }

    public static Error StoreFailed(JobTemplateName name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: StoreFailedCode,
            Message: $"Failed to store the job template '{name}'.");
    }
}
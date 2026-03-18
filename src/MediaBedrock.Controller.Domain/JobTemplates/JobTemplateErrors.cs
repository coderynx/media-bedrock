using Coderynx.Functional.Results.Errors;

namespace MediaBedrock.Controller.Domain.JobTemplates;

public static class JobTemplateErrorCodes
{
    public const string ManifestNotFound = "JobTemplate.ManifestNotFound";
    public const string NotFound = "JobTemplate.NotFound";
    public const string Conflict = "JobTemplate.Conflict";
    public const string ManifestDeserializationFailed = "JobTemplate.ManifestDeserializationFailed";
    public const string InvalidName = "JobTemplate.InvalidName";
    public const string ManifestSerializationFailed = "JobTemplate.ManifestSerializationFailed";
    public const string InvalidAuthor = "JobTemplate.InvalidAuthor";
    public const string InvalidVersion = "JobTemplate.InvalidVersion";
    public const string InvalidId = "JobTemplate.InvalidId";
    public const string InvalidInputId = "JobTemplate.InvalidInputId";
    public const string StoreFailed = "JobTemplate.StoreFailed";
    public const string InvalidStepId = "JobTemplate.InvalidStepId";
    public const string InvalidStepOrder = "JobTemplate.InvalidStepOrder";
}

public static class JobTemplateErrors
{
    public static Error ManifestNotFound(string path)
    {
        return Error.NotFound(
            code: JobTemplateErrorCodes.ManifestNotFound,
            message: $"The job template manifest file '{path}' was not found.");
    }

    public static Error NotFound(JobTemplateName name)
    {
        return Error.NotFound(
            code: JobTemplateErrorCodes.NotFound,
            message: $"The job template '{name}' was not found.");
    }

    public static Error Conflict(JobTemplateName templateName, JobTemplateVersion templateVersion)
    {
        return Error.Conflict(
            code: JobTemplateErrorCodes.Conflict,
            message: $"The job template '{templateName}' with version '{templateVersion}' already exists.");
    }

    public static Error ManifestDeserializationFailed(string path)
    {
        return Error.Custom(
            code: JobTemplateErrorCodes.ManifestDeserializationFailed,
            message: $"Failed to deserialize the job template manifest file '{path}'.");
    }

    public static Error InvalidName(string name)
    {
        return Error.InvalidInput(
            code: JobTemplateErrorCodes.InvalidName,
            message: $"The job template name '{name}' is invalid.");
    }

    public static Error ManifestSerializationFailed(string message)
    {
        return Error.Custom(
            code: JobTemplateErrorCodes.ManifestSerializationFailed,
            message: $"Failed to serialize the job template manifest. {message}");
    }

    public static Error InvalidAuthor(string author)
    {
        return Error.InvalidInput(
            code: JobTemplateErrorCodes.InvalidAuthor,
            message: $"The job template author '{author}' is invalid.");
    }

    public static Error InvalidVersion(string version)
    {
        return Error.InvalidInput(
            code: JobTemplateErrorCodes.InvalidVersion,
            message: $"The job template version '{version}' is invalid.");
    }

    public static Error StoreFailed(JobTemplateName name)
    {
        return Error.InvalidInput(
            code: JobTemplateErrorCodes.StoreFailed,
            message: $"Failed to store the job template '{name}'.");
    }

    public static Error InvalidInputId()
    {
        return Error.InvalidInput(
            code: JobTemplateErrorCodes.InvalidInputId,
            message: "The job template input ID is invalid.");
    }

    public static Error InvalidId()
    {
        return Error.InvalidInput(
            code: JobTemplateErrorCodes.InvalidId,
            message: "The job template ID is invalid.");
    }

    public static Error InvalidStepId()
    {
        return Error.InvalidInput(
            code: JobTemplateErrorCodes.InvalidStepId,
            message: "The job template step ID is invalid.");
    }

    public static Error InvalidStepOrder(uint value)
    {
        return Error.InvalidInput(
            code: JobTemplateErrorCodes.InvalidStepOrder,
            message: $"The job template step order '{value}' is invalid. It must be greater than zero.");
    }
}
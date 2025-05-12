using Coderynx.Functional.Results.Errors;

namespace MediaBedrock.Domain.Jobs.Parameters;

public static class JobParameterErrorCodes
{
    public const string InvalidInput = "JobParameter.InvalidInput";
    public const string InvalidOutput = "JobParameter.InvalidOutput";
    public const string InvalidProperty = "JobParameter.InvalidProperty";
    public const string NotFound = "JobParameter.NotFound";
}

public static class JobParameterErrors
{
    public static Error InputParameterNotFound(string inputName)
    {
        return Error.NotFound(
            code: JobParameterErrorCodes.NotFound,
            message: $"The job input parameter '{inputName}' was not found.");
    }

    public static Error OutputParameterNotFound(string outputName)
    {
        return Error.NotFound(
            code: JobParameterErrorCodes.NotFound,
            message: $"The job output parameter '{outputName}' was not found.");
    }

    public static Error InvalidInputParameter(string inputName)
    {
        return Error.InvalidInput(
            code: JobParameterErrorCodes.InvalidInput,
            message: $"The job input parameter '{inputName}' is invalid.");
    }

    public static Error InvalidOutputParameter(string outputName)
    {
        return Error.InvalidInput(
            code: JobParameterErrorCodes.InvalidOutput,
            message: $"The job output parameter '{outputName}' is invalid.");
    }

    public static Error InvalidPropertyParameter(string propertyName)
    {
        return Error.InvalidInput(
            code: JobParameterErrorCodes.InvalidProperty,
            message: $"The job property parameter '{propertyName}' is invalid.");
    }
}
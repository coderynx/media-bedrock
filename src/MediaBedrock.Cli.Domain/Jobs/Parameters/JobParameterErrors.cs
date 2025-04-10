using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Parameters;

public static class JobParameterErrors
{
    public static Error InputParameterNotFound(string inputName)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "JobParameter.InputParameterNotFound",
            Message:
            $"The job input parameter '{inputName}' was not found. Please check the input name and try again.");
    }

    public static Error OutputParameterNotFound(string outputName)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "JobParameter.OutputParameterNotFound",
            Message:
            $"The job output parameter '{outputName}' was not found. Please check the output name and try again.");
    }

    public static Error InvalidInputParameter(string inputName)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "JobParameter.InvalidInputString",
            Message: $"The job input parameter '{inputName}' is invalid. Please check the format and try again.");
    }

    public static Error InvalidOutputParameter(string outputName)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "JobParameter.InvalidOutputString",
            Message: $"The job output parameter '{outputName}' is invalid. Please check the format and try again.");
    }

    public static Error InvalidPropertyParameter(string propertyName)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "JobParameter.InvalidPropertyString",
            Message: $"The job property parameter '{propertyName}' is invalid. Please check the format and try again.");
    }
}
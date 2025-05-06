using Coderynx.Functional.Results.Errors;

namespace MediaBedrock.Cli.Domain.Processors;

public static class ProcessorErrorCodes
{
    public const string NotFound = "Processor.NotFound";
    public const string InvalidNamespace = "Processor.InvalidNamespace";
    public const string InvalidName = "Processor.InvalidName";
    public const string ExecutionFailed = "Processor.ExecutionFailed";
}

public static class ProcessorErrors
{
    public static Error NotFound(string name)
    {
        return Error.NotFound(
            code: ProcessorErrorCodes.NotFound,
            message: $"The processor '{name}' was not found."
        );
    }

    public static Error InvalidNamespace(string @namespace)
    {
        return Error.InvalidInput(
            code: ProcessorErrorCodes.InvalidNamespace,
            message: $"The processor namespace '{@namespace}' is invalid."
        );
    }

    public static Error InvalidName(string name)
    {
        return Error.InvalidInput(
            code: ProcessorErrorCodes.InvalidName,
            message: $"The processor name '{name}' is invalid."
        );
    }

    public static Error ExecutionFailed(string message)
    {
        return Error.Custom(
            code: ProcessorErrorCodes.ExecutionFailed,
            message: $"The processor execution failed with the following message: {message}"
        );
    }
}
using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Processors;

public static class ProcessorErrors
{
    public static Error NotFound(string name)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: "Processor.NotFound",
            Message: $"The processor '{name}' was not found. Please check the name and try again.");
    }

    public static Result<ProcessorName> InvalidNamespace(string @namespace)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Processor.InvalidNamespace",
            Message:
            $"The processor namespace '{@namespace}' is invalid. It should not be empty or contain invalid characters.");
    }

    public static Error InvalidName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Processor.InvalidName",
            Message: $"The processor name '{name}' is invalid. It should be in the format 'namespace/name'.");
    }
}
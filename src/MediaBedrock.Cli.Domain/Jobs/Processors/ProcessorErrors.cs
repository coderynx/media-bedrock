using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Processors;

public static class ProcessorErrors
{
    public static Error NotFound(string name)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: "Processor.NotFound",
            Message: $"The processor '{name}' was not found. Please check the name and try again.");
    }

    public static Error InvalidName(string name)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Processor.InvalidName",
            Message: $"The processor name '{name}' is invalid. It should be in the format 'namespace/name'.");
    }

    public static Error ProcessingFailed(JobId jobId, ProcessorName processorName)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "Processor.ProcessingFailed",
            Message: $"The processor '{processorName}' failed to process the job with ID '{jobId}'.");
    }
}
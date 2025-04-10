using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.Jobs.Batches;

public static class BatchJobErrors
{
    public static Error InvalidId(Guid id)
    {
        return new Error(
            ResultError: ResultError.InvalidInput,
            Code: "InvalidId",
            Message: $"The provided batch job ID '{id}' is invalid. It cannot be empty or default."
        );
    }

    public static Error DeserializationFailed(string serialized)
    {
        return new Error(
            ResultError: ResultError.Custom,
            Code: "DeserializationFailed",
            Message: $"Failed to deserialize the batch job from the provided string: '{serialized}'."
        );
    }
}
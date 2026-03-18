using Coderynx.Functional.Results.Errors;

namespace MediaBedrock.Controller.Domain.BatchJobs;

public static class BatchJobErrorCodes
{
    public const string InvalidId = "BatchJob.InvalidId";
    public const string DeserializationFailed = "BatchJob.DeserializationFailed";
}

public static class BatchJobErrors
{
    public static Error InvalidId(Guid id)
    {
        return Error.InvalidInput(
            BatchJobErrorCodes.InvalidId,
            message: $"The provided batch job ID '{id}' is invalid. It cannot be empty or default."
        );
    }

    public static Error DeserializationFailed(string serialized)
    {
        return Error.Custom(
            code: BatchJobErrorCodes.DeserializationFailed,
            message: $"Failed to deserialize the batch job from the provided string: '{serialized}'."
        );
    }
}
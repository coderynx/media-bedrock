using Coderynx.Functional.Results.Errors;

namespace MediaBedrock.Domain.JobRuns;

public static class JobRunErrorCodes
{
    public const string InvalidId = "JobRune.InvalidId";
    public const string InvalidStepId = "JobRune.InvalidStepId";
    public const string InvalidTag = "JobRune.InvalidTag";
    public const string NotFound = "JobRune.NotFound";
    public const string NotFoundStep = "JobRune.NotFoundJobStep";
    public const string InvalidStepStatusTransition = "JobRune.InvalidStepStatusTransition";
    public const string UpdatedFailed = "JobRune.UpdateFailed";
    public const string InvalidStepOrder = "JobRune.InvalidStepOrder";
}

public static class JobRunErrors
{
    public static Error InvalidId()
    {
        return Error.InvalidInput(
            code: JobRunErrorCodes.InvalidId,
            message: "The job run ID is invalid.");
    }

    public static Error NotFound(JobRunId jobRunId)
    {
        return Error.NotFound(
            code: JobRunErrorCodes.NotFound,
            message: $"The job run with id {jobRunId} was not found.");
    }

    public static Error NotFound(JobRunStepId jobRunStepId)
    {
        return Error.NotFound(
            code: JobRunErrorCodes.NotFoundStep,
            message: $"The job run step with id {jobRunStepId} was not found.");
    }

    public static Error InvalidStepId()
    {
        return Error.InvalidInput(
            code: JobRunErrorCodes.InvalidStepId,
            message: "The job run step ID is invalid.");
    }

    public static Error InvalidStepStatusTransition(
        JobRunStepStatus currentStatus,
        JobRunStepStatus targetStatus)
    {
        return Error.InvalidInput(
            code: JobRunErrorCodes.InvalidStepStatusTransition,
            message: $"Cannot transition from {currentStatus} to {targetStatus}.");
    }

    public static Error UpdateFailed(JobRunId jobRunId)
    {
        return Error.Custom(
            code: JobRunErrorCodes.UpdatedFailed,
            message: $"Failed to update job run with id {jobRunId}.");
    }

    public static Error InvalidTag(string tag)
    {
        return Error.InvalidInput(
            code: JobRunErrorCodes.InvalidTag,
            message: $"The job run tag '{tag}' is invalid.");
    }

    public static Error InvalidStepOrder(uint value)
    {
        return Error.InvalidInput(
            code: JobRunErrorCodes.InvalidStepOrder,
            message: $"The job run step order '{value}' is invalid. It must be greater than zero.");
    }
}
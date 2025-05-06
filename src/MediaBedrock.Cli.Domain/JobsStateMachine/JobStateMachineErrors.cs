using Coderynx.Functional.Results.Errors;

namespace MediaBedrock.Cli.Domain.JobsStateMachine;

public static class JobStateMachineErrorCodes
{
    public const string InvalidId = "JobStateMachine.InvalidId";
    public const string InvalidStepId = "JobStateMachine.InvalidStepId";
    public const string NotFound = "JobStateMachine.NotFound";
    public const string NotFoundStep = "JobStateMachine.NotFoundJobStep";
    public const string InvalidStepExecutionStatusTransition = "JobStateMachine.InvalidStepExecutionStatusTransition";
}

public static class JobStateMachineErrors
{
    public static Error InvalidId()
    {
        return Error.InvalidInput(
            code: JobStateMachineErrorCodes.InvalidId,
            message: "The job state machine ID is invalid.");
    }

    public static Error NotFound(JobStateMachineId jobStateMachineId)
    {
        return Error.NotFound(
            code: JobStateMachineErrorCodes.NotFound,
            message: $"The job state machine with id {jobStateMachineId} was not found.");
    }

    public static Error NotFound(JobStepStateMachineId jobStepStateMachineId)
    {
        return Error.NotFound(
            code: JobStateMachineErrorCodes.NotFoundStep,
            message: $"The job step state machine with id {jobStepStateMachineId} was not found.");
    }

    public static Error InvalidStepId()
    {
        return Error.InvalidInput(
            code: JobStateMachineErrorCodes.InvalidStepId,
            message: "The job step state machine ID is invalid.");
    }
    
    public static Error InvalidStepExecutionStatusTransition(
        JobStepExecutionStatus currentStatus,
        JobStepExecutionStatus targetStatus)
    {
        return Error.InvalidInput(
            code: JobStateMachineErrorCodes.InvalidStepExecutionStatusTransition,
            message: $"Cannot transition from {currentStatus} to {targetStatus}.");
    }
}
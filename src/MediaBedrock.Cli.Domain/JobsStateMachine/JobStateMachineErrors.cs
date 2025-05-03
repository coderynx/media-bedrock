using Coderynx.Functional;
using Coderynx.Functional.Results;

namespace MediaBedrock.Cli.Domain.JobsStateMachine;

public static class JobStateMachineErrors
{
    public const string InvalidStepIdCode = "JobStateMachine.InvalidStepId";
    public const string NotFoundCode = "JobStateMachine.NotFound";
    public const string NotFoundStepCode = "JobStateMachine.NotFoundJobStep";

    public static Error NotFound(JobStateMachineId jobStateMachineId)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: NotFoundCode,
            Message: "The job state machine with id {JobStateMachineId} was not found.");
    }

    public static Error NotFound(JobStepStateMachineId jobStepStateMachineId)
    {
        return new Error(
            ResultError: ResultError.NotFound,
            Code: NotFoundStepCode,
            Message: $"The job step state machine with id {jobStepStateMachineId} was not found.");
    }
}
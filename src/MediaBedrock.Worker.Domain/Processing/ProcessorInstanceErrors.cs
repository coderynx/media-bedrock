using Coderynx.Functional.Results.Errors;
using MediaBedrock.Worker.Domain.Processing.ValueObjects;

namespace MediaBedrock.Worker.Domain.Processing;

public static class ProcessorInstanceErrorCodes
{
    public const string InvalidId = "ProcessorInstance.InvalidId";
    public const string InvalidJobRunStepId = "ProcessorInstance.InvalidJobRunStepId";
    public const string InvalidProcessorNamespace = "ProcessorInstance.InvalidProcessorNamespace";
    public const string InvalidProcessorName = "ProcessorInstance.InvalidProcessorName";
    public const string InvalidInputName = "ProcessorInstance.InvalidInputName";
    public const string InvalidOutputName = "ProcessorInstance.InvalidOutputName";
    public const string InvalidPropertyName = "ProcessorInstance.InvalidPropertyName";
    public const string InvalidStatusTransition = "ProcessorInstance.InvalidStatusTransition";
    public const string RunFailed = "ProcessorInstance.RunFailed";
    public const string NotFound = "ProcessorInstance.NotFound";
}

public static class ProcessorInstanceErrors
{
    public static Error InvalidId(Guid value)
    {
        return Error.InvalidInput(
            code: ProcessorInstanceErrorCodes.InvalidId,
            message: $"The processor instance ID '{value}' is invalid.");
    }
    
    public static Error InvalidJobRunStepId(Guid value)
    {
        return Error.InvalidInput(
            code: ProcessorInstanceErrorCodes.InvalidJobRunStepId,
            message: $"The processor instance job run step ID '{value}' is invalid.");
    }

    public static Error InvalidProcessorNamespace(string value)
    {
        return Error.InvalidInput(
            code: ProcessorInstanceErrorCodes.InvalidProcessorNamespace,
            message: $"The processor namespace '{value}' is invalid.");       
    }
    
    public static Error InvalidProcessorName(string value)
    {
        return Error.InvalidInput(
            code: ProcessorInstanceErrorCodes.InvalidProcessorName,
            message: $"The processor name '{value}' is invalid.");
    }

    public static Error InvalidInputName(string value)
    {
        return Error.InvalidInput(
            code: ProcessorInstanceErrorCodes.InvalidInputName,
            message: $"The processor instance input name '{value}' is invalid.");
    }

    public static Error InvalidOutputName(string value)
    {
        return Error.InvalidInput(
            code: ProcessorInstanceErrorCodes.InvalidOutputName,
            message: $"The processor instance output name '{value}' is invalid.");
    }

    public static Error InvalidPropertyName(string value)
    {
        return Error.InvalidInput(
            code: ProcessorInstanceErrorCodes.InvalidPropertyName,
            message: $"The processor instance property name '{value}' is invalid.");
    }

    public static Error InvalidStatusTransition(
        ProcessorInstanceStatus currentStatus,
        ProcessorInstanceStatus targetStatus)
    {
        return Error.InvalidOperation(
            code: ProcessorInstanceErrorCodes.InvalidStatusTransition,
            message: $"Cannot transition from {currentStatus} to {targetStatus}.");
    }

    public static Error RunFailed(string message)
    {
        return Error.Custom(
            code: ProcessorInstanceErrorCodes.RunFailed,
            message: $"The processor instance execution failed with the following message: {message}");
    }
    
    public static Error NotFound(ProcessorInstanceId value)
    {
        return Error.NotFound(
            code: ProcessorInstanceErrorCodes.NotFound,
            message: $"The processor instance '{value}' was not found.");
    }
}
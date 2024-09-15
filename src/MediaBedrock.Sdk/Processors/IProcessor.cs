namespace MediaBedrock.Sdk.Processors;

public sealed record ProcessorResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;

    public static ProcessorResult Success()
    {
        return new ProcessorResult
        {
            IsSuccess = true
        };
    }

    public static ProcessorResult Failure(string message)
    {
        return new ProcessorResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}

public interface IProcessor
{
    Task<ProcessorResult> ProcessAsync(ProcessorContext context, CancellationToken ct = default);
}
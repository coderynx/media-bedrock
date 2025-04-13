namespace MediaBedrock.Sdk.Processors;

public sealed record ProcessorResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public Exception? Exception { get; init; }

    public static ProcessorResult Success()
    {
        return new ProcessorResult
        {
            IsSuccess = true
        };
    }

    public static ProcessorResult Failure(string message, Exception? exception = null)
    {
        return new ProcessorResult
        {
            IsSuccess = false,
            Message = message,
            Exception = exception
        };
    }
}

public interface IProcessor
{
    Task<ProcessorResult> ProcessAsync(ProcessorContext context, CancellationToken ct = default);
}
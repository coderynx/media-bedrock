namespace MediaBedrock.Worker.Sdk.Processors;

public interface IProcessor
{
    Task<ProcessorResult> ProcessAsync(ProcessorContext context, CancellationToken cancellationToken = default);
}
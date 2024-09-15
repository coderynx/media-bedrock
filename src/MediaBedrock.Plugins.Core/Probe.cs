using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Plugins.Core;

[Processor("core", "probe")]
public sealed class Probe(ILogger<Probe> logger) : IProcessor
{
    public Task<ProcessorResult> ProcessAsync(ProcessorContext context, CancellationToken ct = default)
    {
        logger.LogInformation("Received ProcessorContext: {@Context}", context);

        return Task.FromResult(ProcessorResult.Success());
    }
}
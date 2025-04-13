using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Plugins.Core;

[Processor("core", "probe")]
public sealed class Probe : IProcessor
{
    public Task<ProcessorResult> ProcessAsync(ProcessorContext context, CancellationToken ct = default)
    {
        context.Logger.LogInformation("Received ProcessorContext: {@Context}", context);

        return Task.FromResult(ProcessorResult.Success());
    }
}
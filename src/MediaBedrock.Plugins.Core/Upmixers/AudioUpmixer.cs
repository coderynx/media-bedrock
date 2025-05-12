using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Plugins.Core.Upmixers;

[Processor("core", "audio_upmixer")]
public sealed class AudioUpmixer : IProcessor
{
    public Task<ProcessorResult> ProcessAsync(ProcessorContext context, CancellationToken cancellationToken = default)
    {
        const string targetChannelLayoutKey = "target_channel_layout";
        const string inputKey = "input";
        const string outputKey = "output";

        var targetChannelLayout = context.GetPropertyRequired(targetChannelLayoutKey);
        switch (targetChannelLayout.Value)
        {
            case "7.1.4":
                var input = context.GetInputRequired(inputKey);
                var output = context.GetOutputRequired(outputKey);

                var upmixer = new SevenOneFourUpmixer();

                context.Logger.LogInformation("Starting upmixing process");

                try
                {
                    upmixer.Upmix(input.GetAsFilePath(), output.GetAsFilePath());
                }
                catch (Exception e)
                {
                    context.Logger.LogError(e, "Failed to upmix audio");
                    return Task.FromResult(ProcessorResult.Failure($"Failed to upmix audio: {e.Message}"));
                }

                break;

            case null:
                context.Logger.LogError("{Key} property is required but not provided", targetChannelLayoutKey);
                return Task.FromResult(
                    ProcessorResult.Failure("TargetChannelLayout property is required but not provided"));
        }

        context.Logger.LogInformation("Upmixing process completed successfully");
        return Task.FromResult(ProcessorResult.Success());
    }
}
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Plugins.FFmpeg;

[Processor("ffmpeg", "wave_resampler")]
public sealed class WaveResampler : IProcessor
{
    public async Task<ProcessorResult> ProcessAsync(
        ProcessorContext context,
        CancellationToken cancellationToken = default)
    {
        const string inputKey = "input";
        const string outputKey = "output";
        const string targetSampleRateKey = "target_sample_rate";
        const string executablePathKey = "executable_path";

        var inputTrack = context.GetInput(inputKey);
        if (inputTrack is null)
        {
            return ProcessorResult.Failure($"{inputKey} track not found");
        }

        if (!inputTrack.MediaInformation.Format.Equals("Wave"))
        {
            context.Logger.LogError("Input track is not of type Wave");
            return ProcessorResult.Failure("Input track is not of type Wave");
        }

        var outputTrack = context.GetOutput(outputKey);
        if (outputTrack is null)
        {
            return ProcessorResult.Failure($"{outputKey} track not found");
        }

        var targetSampleRate = context.GetPropertyRequired(targetSampleRateKey)
            .GetValue(sampleRate => int.TryParse(sampleRate, out var value) ? value : 0);

        var ffmpegPath = context.GetProperty(executablePathKey)?.GetValue();
        var wrapper = new FFmpegWrapper(ffmpegPath ?? "ffmpeg");

        context.Logger.LogInformation("Resampling {Input} to {Output} with sample rate {SampleRate}",
            inputTrack,
            outputTrack,
            targetSampleRate);

        try
        {
            await wrapper.ExecuteFFmpegCommandAsync(
                inputs: [inputTrack.GetAsFilePath()],
                outputs: [outputTrack.GetAsFilePath()],
                arguments: $"-ar {targetSampleRate} -f wav",
                log: message =>
                {
                    if (message.Message.Contains("Error", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Logger.LogError("Received error from FFmpeg {Message}", message);
                    }
                });
        }
        catch (Exception exception)
        {
            return ProcessorResult.Failure("Failed to resample audio", exception);
        }

        context.Logger.LogInformation("Resampled {Input} to {Output} with sample rate {SampleRate}",
            inputTrack,
            outputTrack,
            targetSampleRate);

        return ProcessorResult.Success();
    }
}
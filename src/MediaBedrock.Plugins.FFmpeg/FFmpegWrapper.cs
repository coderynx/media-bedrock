using System.Diagnostics;

namespace MediaBedrock.Plugins.FFmpeg;

internal sealed record FFmpegMessage(string Message)
{
    public override string ToString()
    {
        return Message;
    }
}

internal sealed class FFmpegWrapper
{
    private readonly string _ffmpegPath;

    public FFmpegWrapper(string ffmpegPath)
    {
        if (string.IsNullOrWhiteSpace(ffmpegPath))
        {
            throw new ArgumentException("FFmpeg path cannot be null or empty.", nameof(ffmpegPath));
        }

        _ffmpegPath = ffmpegPath;
    }

    public async Task ExecuteFFmpegCommandAsync(
        string[] inputs,
        string[] outputs,
        string arguments,
        Action<FFmpegMessage>? log = null)
    {
        if (inputs is null || inputs.Length is 0)
        {
            throw new ArgumentException("At least one input file is required.", nameof(inputs));
        }

        if (outputs is null || outputs.Length is 0)
        {
            throw new ArgumentException("At least one output file is required.", nameof(outputs));
        }

        if (string.IsNullOrWhiteSpace(arguments))
        {
            throw new ArgumentException("FFmpeg arguments cannot be null or empty.", nameof(arguments));
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = _ffmpegPath,
                Arguments = $"-i \"{string.Join("\" -i \"", inputs)}\" {arguments} \"{string.Join("\" \"", outputs)}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (_, args) =>
        {
            if (args.Data is null)
            {
                return;
            }

            var message = new FFmpegMessage(args.Data);
            log?.Invoke(message);
        };

        process.ErrorDataReceived += (_, args) =>
        {
            if (args.Data is null)
            {
                return;
            }

            var message = new FFmpegMessage(args.Data);
            log?.Invoke(message);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        await process.WaitForExitAsync();

        if (process.ExitCode is not 0)
        {
            throw new InvalidOperationException($"FFmpeg process failed with exit code {process.ExitCode}.");
        }
    }
}
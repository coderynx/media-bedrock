using System.Diagnostics;
using System.Text;
using MediaBedrock.Dolby.EncodingEngine.Messages;
using MediaBedrock.Dolby.Jobs.Models;
using MediaBedrock.Dolby.Jobs.Serializers;

namespace MediaBedrock.Dolby.EncodingEngine;

public sealed class DolbyEncodingEngine : IDolbyEncodingEngine
{
    public DolbyEncodingEngine(string path, bool useWine = false)
    {
        IsUsingWine = useWine;
        Path = path;

        Initialize();
    }

    private bool IsInitialized => !string.IsNullOrEmpty(Version);

    public bool IsUsingWine { get; }
    public string Path { get; }
    public string ExecutablePath => System.IO.Path.Combine(Path, "dee.exe");
    public string Version { get; private set; } = string.Empty;

    public async Task ProcessJobAsync(JobDefinition job, Action<DolbyEncodingEngineMessage>? onStatusChange = null)
    {
        if (!IsInitialized)
        {
            throw new DolbyEncodingEngineException("The encoding engine is not initialized.");
        }

        var dto = job.ToDto();
        var json = JobDefinitionDtoSerializer.Serialize(dto);

        var tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
        Directory.CreateDirectory(tempPath);

        var fileName = $"{Guid.CreateVersion7().ToString()}.json";
        var filePath = System.IO.Path.Combine(tempPath, fileName);

        await File.WriteAllTextAsync(filePath, json);

        var process = CreateProcess($"--json {filePath} --progress", (_, args) =>
        {
            if (args.Data is null)
            {
                return;
            }

            var logData = DolbyEncodingEngineLogLine.Create(args.Data);
            if (logData is null)
            {
                return;
            }

            var message = DolbyEncodingEngineMessageFactory.Create(logData);
            onStatusChange?.Invoke(message);
        });

        process.Start();
        process.BeginOutputReadLine();
        await process.WaitForExitAsync();

#if !DEBUG
      File.Delete(filePath);
#endif
    }

    private Process CreateProcess(string arguments = "", DataReceivedEventHandler? outputHandler = null)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = Path,
                FileName = IsUsingWine ? "wine" : "dee.exe",
                Arguments = IsUsingWine ? $"dee.exe {arguments}" : $" {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };

        if (outputHandler is not null)
        {
            process.OutputDataReceived += outputHandler;
        }

        return process;
    }

    private void Initialize()
    {
        if (!Directory.Exists(Path))
        {
            throw new DolbyEncodingEngineException($"The specified encoding engine path {Path} does not exist.");
        }

        if (!File.Exists(ExecutablePath))
        {
            throw new DolbyEncodingEngineException(
                $"The specified encoding engine path {ExecutablePath} does not contain the required executable.");
        }

        var outputBuilder = new StringBuilder();

        var process = CreateProcess(outputHandler: (_, args) =>
        {
            if (args.Data is not null)
            {
                outputBuilder.AppendLine(args.Data);
            }
        });

        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();

        try
        {
            var output = outputBuilder.ToString();
            var parts = output.Split(',');
            if (parts.Length <= 1)
            {
                throw new DolbyEncodingEngineException(
                    "Failed to parse the encoding engine version. The output format is invalid.");
            }
            Version = parts[1]
                .Replace("Version", string.Empty)
                .Trim();
        }
        catch (Exception e)
        {
            throw new DolbyEncodingEngineException("Failed to initialize the encoding engine.", e);
        }
    }
}
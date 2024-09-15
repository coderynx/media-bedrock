using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using MediaBedrock.Dolby.Jobs.Models;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Dolby.EncodingEngine;

public interface IDolbyEncodingEngine
{
    bool IsUsingWine { get; }
    string Path { get; }
    string ExecutablePath { get; }
    string Version { get; }
    Task ProcessJobAsync(JobDefinition job, Action<ProcessingStatus>? onStatusChange = null);
}

public sealed partial class DolbyEncodingEngine : IDolbyEncodingEngine
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly ILogger<DolbyEncodingEngine>? _logger;

    public DolbyEncodingEngine(string path, bool useWine = false)
    {
        IsUsingWine = useWine;
        Path = path;

        Initialize();
    }

    public DolbyEncodingEngine(string path, bool useWine, ILoggerFactory loggerFactory) : this(path, useWine)
    {
        _logger = loggerFactory.CreateLogger<DolbyEncodingEngine>();
    }

    private bool IsInitialized => !string.IsNullOrEmpty(Version);

    public bool IsUsingWine { get; }
    public string Path { get; }
    public string ExecutablePath => System.IO.Path.Combine(Path, "dee.exe");
    public string Version { get; private set; } = string.Empty;

    public async Task ProcessJobAsync(JobDefinition job, Action<ProcessingStatus>? onStatusChange = null)
    {
        if (!IsInitialized)
        {
            _logger?.LogError("The encoding engine is not initialized.");
            throw new DolbyEncodingEngineException("The encoding engine is not initialized.");
        }

        _logger?.LogInformation("Start processing job {@Job}", job);

        var dto = job.ToDto();
        var json = JsonSerializer.Serialize(dto, _jsonOptions);

        var tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp");
        Directory.CreateDirectory(tempPath);

        var filePath = System.IO.Path.Combine(tempPath, "job.json");

        await File.WriteAllTextAsync(filePath, json);

        var status = new ProcessingStatus("initializing");

        var process = CreateProcess($"--json {filePath} --progress", (_, args) =>
        {
            if (args.Data is null)
            {
                return;
            }

            LogLine(args.Data);

            if (onStatusChange is null)
            {
                return;
            }

            var match = ProgressRegex().Match(args.Data);
            if (!match.Success)
            {
                onStatusChange(status);
                return;
            }

            status = new ProcessingStatus
            {
                Stage = match.Groups["stage"].Value,
                StageName = match.Groups["stageName"].Value,
                Step = match.Groups["step"].Value,
                StageProgress = double.Parse(match.Groups["stageProgress"].Value),
                OverallProgress = double.Parse(match.Groups["overallProgress"].Value)
            };

            onStatusChange(status);
        });

        process.Start();
        process.BeginOutputReadLine();
        await process.WaitForExitAsync();

#if !DEBUG
      File.Delete(filePath);
#endif

        _logger?.LogInformation("Finished processing job {@Job}", job);
    }

    private void LogLine(string line)
    {
        var logMatch = LogRegex().Match(line);

        if (!logMatch.Success)
        {
            return;
        }

        var logLevel = logMatch.Groups["logLevel"].Value;
        var category = logMatch.Groups["category"].Value;
        var message = logMatch.Groups["message"].Value;

        var logLevelType = logLevel switch
        {
            "INFO" => LogLevel.Information,
            "INTERNAL_INFO" => LogLevel.Information,
            "WARNING" => LogLevel.Warning,
            "ERROR" => LogLevel.Error,
            _ => LogLevel.None
        };

        _logger?.Log(logLevelType, "Dolby Encoding Engine [{Category}]: {Message}", category, message);
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
            var exception = new DolbyEncodingEngineException("The specified encoding engine path does not exist.");
            _logger?.LogError(exception, "The specified encoding engine path does not exist.");
        }

        if (!File.Exists(ExecutablePath))
        {
            var exception = new DolbyEncodingEngineException(
                message: "The specified encoding engine path does not contain the required executable.");
            _logger?.LogError(
                exception,
                "The specified encoding engine path does not contain the required executable.");
        }

        var output = string.Empty;

        var process = CreateProcess(outputHandler: (_, args) =>
        {
            if (args.Data is not null)
            {
                output += args.Data;
            }
        });

        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();

        try
        {
            Version = output.Split(",")[1]
                .Replace("Version", string.Empty)
                .Trim();
        }
        catch (Exception e)
        {
            _logger?.LogError(e, "Failed to initialize the encoding engine.");
            throw new DolbyEncodingEngineException("Failed to initialize the encoding engine.", e);
        }

        _logger?.LogInformation("Initialized Dolby Encoding Engine version {Version}", Version);
    }

    [GeneratedRegex("Stage: (?<stage>.*?)," +
                    "Stage name: (?<stageName>.*?)," +
                    "Step: (?<step>.*?)," +
                    "Stage progress: (?<stageProgress>[0-9.]+)," +
                    "Overall progress: (?<overallProgress>[0-9.]+)(?=\\.)")]
    private static partial Regex ProgressRegex();

    [GeneratedRegex(@"\[(?<timestamp>[^\]]+)\] (?<logLevel>\w+): (?<category>\w+): (?<message>.+)")]
    private static partial Regex LogRegex();
}

public sealed record ProcessingStatus
{
    internal ProcessingStatus(string stage)
    {
        Stage = stage;
    }

    internal ProcessingStatus()
    {
    }

    public string Stage { get; init; } = string.Empty;
    public string StageName { get; init; } = string.Empty;
    public string Step { get; init; } = string.Empty;
    public double StageProgress { get; init; }
    public double OverallProgress { get; init; }
}
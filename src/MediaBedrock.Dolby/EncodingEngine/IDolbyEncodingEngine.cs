using MediaBedrock.Dolby.Jobs.Models;

namespace MediaBedrock.Dolby.EncodingEngine;

public interface IDolbyEncodingEngine
{
    bool IsUsingWine { get; }
    string Path { get; }
    string ExecutablePath { get; }
    string Version { get; }
    Task ProcessJobAsync(JobDefinition job, Action<DolbyEncodingEngineMessage>? onStatusChange = null);
}
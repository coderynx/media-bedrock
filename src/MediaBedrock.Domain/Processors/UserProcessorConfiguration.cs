namespace MediaBedrock.Domain.Processors;

public record UserProcessorConfiguration
{
    public required string Name { get; init; }
    public Dictionary<string, string?> Settings { get; init; } = new();
}

public sealed record ProcessorConfiguration : UserProcessorConfiguration
{
    public required string PluginPath { get; init; }

    public static ProcessorConfiguration Create(string processorName, string pluginPath)
    {
        return new ProcessorConfiguration
        {
            Name = processorName,
            Settings = [],
            PluginPath = pluginPath
        };
    }

    public static ProcessorConfiguration Create(UserProcessorConfiguration configuration, string pluginPath)
    {
        return new ProcessorConfiguration
        {
            Name = configuration.Name,
            Settings = configuration.Settings,
            PluginPath = pluginPath
        };
    }
}
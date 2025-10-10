using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Coderynx.Functional.Results;
using MediaBedrock.Domain.Processors;
using MediaBedrock.Infrastructure.Plugins.Interfaces;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Infrastructure.Plugins;

public record UserPluginConfiguration
{
    public List<UserProcessorConfiguration> Processors { get; init; } = [];
}

internal sealed record PluginEntry(
    Assembly Assembly,
    string PluginPath,
    UserPluginConfiguration Configuration);

public sealed class PluginsManager(ILogger<PluginsManager> logger) : IPluginsManager
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private IServiceProvider? _serviceProvider;

    public void Initialize()
    {
        var serviceCollection = new ServiceCollection();

        var plugins = LoadPlugins();

        foreach (var plugin in plugins)
        {
            var processorTypes = plugin.Assembly.GetTypes()
                .Where(t => typeof(IProcessor).IsAssignableFrom(t) && !t.IsInterface);

            foreach (var type in processorTypes)
            {
                var processorInfo = GetProcessorInfo(type);
                var fullName = $"{processorInfo.Namespace}/{processorInfo.Name}";

                serviceCollection.AddKeyedTransient(
                    fullName,
                    (sp, _) => (IProcessor)ActivatorUtilities.CreateInstance(sp, type));

                logger.LogInformation("Registered processor {ProcessorFullName}", fullName);

                var configuration = ProcessorConfiguration.Create(fullName, plugin.PluginPath);
                serviceCollection.AddKeyedSingleton(configuration.Name, configuration);

                logger.LogInformation("Registered processor configuration {ProcessorConfigurationName}",
                    configuration.Name);
            }

            foreach (var processorConfiguration in plugin.Configuration.Processors)
            {
                var configuration = ProcessorConfiguration.Create(processorConfiguration, plugin.PluginPath);

                serviceCollection.AddKeyedSingleton(processorConfiguration.Name, configuration);
            }
        }

        _serviceProvider = serviceCollection.BuildServiceProvider();

        logger.LogInformation("Plugins loaded");
    }

    public Result<TComponent> ResolveComponent<TComponent>(string name) where TComponent : class
    {
        if (_serviceProvider is null)
        {
            return PluginErrors.ContainerNotInitialized;
        }

        var service = _serviceProvider.GetKeyedService<TComponent>(name);

        return service is null
            ? ProcessorErrors.NotFound(name)
            : Result.Found(service);
    }

    public async ValueTask DisposeAsync()
    {
        if (_serviceProvider is not null)
        {
            switch (_serviceProvider)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
    }

    private static (string Namespace, string Name) GetProcessorInfo(Type type)
    {
        var attribute = type.GetCustomAttribute<ProcessorAttribute>();

        if (attribute is null)
        {
            throw new InvalidOperationException($"Type {type.Name} does not have a {nameof(ProcessorAttribute)}");
        }

        return (attribute.Namespace, attribute.Name);
    }

    private List<PluginEntry> LoadPlugins()
    {
        const string librariesPattern = "MediaBedrock.Plugins.*.dll";
        const string configurationsPattern = "pluginsettings.json";

        var pluginDirectory = Path.Combine(AppContext.BaseDirectory, "Plugins");

        if (!Directory.Exists(pluginDirectory))
        {
            Directory.CreateDirectory(pluginDirectory);
        }

        var directories = Directory.GetDirectories(pluginDirectory, "*", SearchOption.AllDirectories);
        var pluginEntries = new List<PluginEntry>();

        foreach (var directory in directories)
        {
            var libraryFile = Directory.GetFiles(directory, librariesPattern, SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            if (libraryFile is null)
            {
                continue;
            }

            var configurationFile = Directory.GetFiles(directory, configurationsPattern, SearchOption.TopDirectoryOnly)
                .FirstOrDefault();

            var pluginConfiguration = new UserPluginConfiguration();
            if (configurationFile is not null)
            {
                var json = File.ReadAllText(configurationFile);

                pluginConfiguration = JsonSerializer.Deserialize<UserPluginConfiguration>(json, _jsonOptions);
                if (pluginConfiguration is null)
                {
                    // TODO: Add error handling.
                    logger.LogError("Failed to load configuration for plugin: {PluginName}", libraryFile);
                    continue;
                }
            }

            var pluginEntry = new PluginEntry(
                Assembly.LoadFrom(libraryFile),
                directory,
                pluginConfiguration);

            pluginEntries.Add(pluginEntry);
        }

        return pluginEntries;
    }
}
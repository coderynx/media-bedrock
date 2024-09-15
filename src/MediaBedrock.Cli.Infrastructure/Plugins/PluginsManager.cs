using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Coderynx.Functional.Result;
using MediaBedrock.Cli.Domain.Jobs.Processors;
using MediaBedrock.Cli.Infrastructure.Plugins.Interfaces;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MediaBedrock.Cli.Infrastructure.Plugins;

public sealed record PluginConfiguration
{
    public List<ProcessorConfiguration> Processors { get; init; } = [];
}

internal sealed record PluginEntry(Assembly Assembly, PluginConfiguration Configuration);

public sealed class PluginsManager(ILogger<PluginsManager> logger, IConfiguration configuration) : IPluginsManager
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        Converters =
        {
            new DictionaryStringObjectJsonConverter(),
            new JsonStringEnumConverter()
        }
    };

    private IContainer _container = null!;

    public void Initialize()
    {
        var builder = new ContainerBuilder();

        var coreServices = AddCoreServices();
        builder.Populate(coreServices);

        var plugins = LoadPlugins();

        foreach (var plugin in plugins)
        {
            var processorTypes = plugin.Assembly.GetTypes()
                .Where(t => typeof(IProcessor).IsAssignableFrom(t) && !t.IsInterface);

            foreach (var type in processorTypes)
            {
                var processorInfo = GetProcessorInfo(type);
                var fullName = $"{processorInfo.Namespace}/{processorInfo.Name}";

                builder.RegisterType(type).Named<IProcessor>(fullName);
                logger.LogInformation("Registered processor {ProcessorFullName}", fullName);
            }

            foreach (var processorConfiguration in plugin.Configuration.Processors)
            {
                builder.RegisterInstance(processorConfiguration)
                    .Named<ProcessorConfiguration>(processorConfiguration.Name);
                logger.LogInformation("Registered processor configuration {ProcessorConfigurationName}",
                    processorConfiguration.Name);
            }
        }

        _container = builder.Build();

        logger.LogInformation("Plugins loaded");
    }

    public Result<TComponent> ResolveComponent<TComponent>(string componentName) where TComponent : class
    {
        return _container.TryResolveNamed<TComponent>(componentName, out var adapter)
            ? Result.Found(adapter)
            : ProcessorErrors.NotFound(componentName);
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
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

    private ServiceCollection AddCoreServices()
    {
        var collection = new ServiceCollection();

        collection.AddLogging(loggingBuilder =>
        {
            var serilog = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            loggingBuilder.AddSerilog(serilog);
        });

        return collection;
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

            var pluginConfiguration = new PluginConfiguration();
            if (configurationFile is not null)
            {
                var json = File.ReadAllText(configurationFile);

                pluginConfiguration = JsonSerializer.Deserialize<PluginConfiguration>(json, _jsonOptions);
                if (pluginConfiguration is null)
                {
                    // TODO: Add error handling.
                    logger.LogError("Failed to load configuration for plugin: {PluginName}", libraryFile);
                    continue;
                }
            }

            var pluginEntry = new PluginEntry(Assembly.LoadFrom(libraryFile), pluginConfiguration);
            pluginEntries.Add(pluginEntry);
        }

        return pluginEntries;
    }
}
using Coderynx.Functional.Results;
using MediaBedrock.Domain.Processors;
using MediaBedrock.Domain.Processors.Interfaces;
using MediaBedrock.Infrastructure.Plugins.Interfaces;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Infrastructure.Plugins;

public sealed class ProcessorProvider(IPluginsManager pluginsManager) : IProcessorProvider
{
    public Result<IProcessor> ResolveProcessor(ProcessorName name)
    {
        var resolveComponent = pluginsManager.ResolveComponent<IProcessor>(name.ToString());

        return resolveComponent.IsFailure
            ? resolveComponent.Error
            : Result.Found(resolveComponent.Value);
    }

    public Result<ProcessorConfiguration> ResolveConfiguration(ProcessorName name)
    {
        var resolveComponent = pluginsManager.ResolveComponent<ProcessorConfiguration>(name.ToString());

        return resolveComponent.IsFailure
            ? resolveComponent.Error
            : Result.Found(resolveComponent.Value);
    }
}
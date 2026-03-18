using Coderynx.Functional.Results;
using MediaBedrock.Worker.Application.Processing.Interfaces;
using MediaBedrock.Worker.Domain.Processing;
using MediaBedrock.Worker.Domain.Processing.ValueObjects;
using MediaBedrock.Worker.Infrastructure.Plugins.Interfaces;
using MediaBedrock.Worker.Sdk.Processors;

namespace MediaBedrock.Worker.Infrastructure.Processing;

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
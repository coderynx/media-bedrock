using Coderynx.Functional.Results;
using MediaBedrock.Worker.Application.Processing.Interfaces;
using MediaBedrock.Worker.Domain.Processing.Entities;
using MediaBedrock.Worker.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Worker.Application.Processing;

public sealed class ProcessorContextFactory(IProcessorProvider processorProvider, ILoggerFactory loggerFactory)
    : IProcessorContextFactory
{
    public Result<ProcessorContext> Create(Type processorType, ProcessorInstance processorInstance)
    {
        var properties = processorInstance.Properties
            .Select(p => new ProcessorProperty(p.Name, p.Value))
            .ToList();

        var processorConfiguration = processorProvider.ResolveConfiguration(processorInstance.ProcessorName);
        if (processorConfiguration.IsFailure)
        {
            return processorConfiguration.Error;
        }

        properties.AddRange(
            processorConfiguration.Value.Settings.Select(p => new ProcessorProperty(p.Key, p.Value))
        );

        var processorInputs = new List<ProcessorInput>();
        foreach (var input in processorInstance.Inputs)
        {
            var mediaInformation = new MediaInformation(input.AssetInformation.Format);
            processorInputs.Add(new ProcessorInput(input.Name, input.Uri, mediaInformation));
        }

        var processorOutputs = processorInstance.Outputs
            .Select(output => new ProcessorOutput(name: output.Name, assetName: output.Name, uri: output.Uri))
            .ToList();

        var context = ProcessorContext.Create(
            logger: loggerFactory.CreateLogger(processorType),
            pluginPath: processorConfiguration.Value.PluginPath,
            inputs: processorInputs,
            outputs: processorOutputs,
            properties: properties);
        
        return Result.Created(context);
    }
}
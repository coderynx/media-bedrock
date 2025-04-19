using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Assets;
using MediaBedrock.Cli.Domain.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Cli.Application.Jobs;

public sealed class ProcessorContextFactory(
    IProcessorProvider processorProvider,
    ILoggerFactory loggerFactory) : IProcessorContextFactory
{
    public Result<ProcessorContext> Create(Type processorType, JobId jobId, JobStep step, JobAssetsPool assetsPool)
    {
        var properties = step.Properties.Select(p => new ProcessorProperty(p.Name, p.Value)).ToList();

        var processorConfiguration = processorProvider.ResolveConfiguration(step.ProcessorName);
        if (processorConfiguration.IsSuccess)
        {
            properties.AddRange(
                processorConfiguration.Value.Settings.Select(p => new ProcessorProperty(p.Key, p.Value))
            );
        }

        var processorInputs = new List<ProcessorInput>();
        foreach (var input in step.Sinks)
        {
            var resolveAsset = assetsPool.ResolveAsset(input.AssetName);
            if (!resolveAsset.IsSome)
            {
                return JobAssetErrors.AssetNotFound(input.AssetName);
            }

            var asset = resolveAsset.ValueOrThrow();
            if (asset.Uri is null)
            {
                return JobAssetErrors.AssetNotAvailable(input.AssetName);
            }

            var processorInput = new ProcessorInput(input.Name, asset.Uri, asset.MediaInformation!);
            processorInputs.Add(processorInput);
        }

        var processorOutputs = new List<ProcessorOutput>();
        foreach (var output in step.Sources)
        {
            var resolveAsset = assetsPool.ResolveAsset(output.AssetName, JobAssetKind.Output);

            if (resolveAsset.IsSome)
            {
                var jobOutput = new ProcessorOutput(
                    name: output.Name,
                    assetName: output.AssetName,
                    uri: resolveAsset.ValueOrThrow().Uri!);

                processorOutputs.Add(jobOutput);
                continue;
            }

            var tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "temp", jobId.ToString());
            Directory.CreateDirectory(tempPath);

            var processorOutput = new ProcessorOutput(
                name: output.Name,
                assetName: output.AssetName,
                uri: Path.Combine(tempPath, output.AssetName));

            processorOutputs.Add(processorOutput);
        }

        var context = ProcessorContext.Create(
            logger: loggerFactory.CreateLogger(processorType),
            inputs: processorInputs,
            outputs: processorOutputs,
            properties: properties);
        return Result.Created(context);
    }
}
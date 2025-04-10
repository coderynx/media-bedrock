using Coderynx.Functional.Results;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Assets;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Cli.Application.Jobs;

public sealed class ProcessorContextFactory(IProcessorProvider processorProvider) : IProcessorContextFactory
{
    public Result<ProcessorContext> Create(JobId jobId, JobStep step, JobAssetsPool assetsPool)
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

        var context = ProcessorContext.Create(processorInputs, processorOutputs, properties);
        return Result.Created(context);
    }
}
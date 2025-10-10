using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Steps;
using MediaBedrock.Domain.Processors.Interfaces;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Application.JobRuns.Factories;

public sealed class ProcessorContextFactory(
    IProcessorProvider processorProvider,
    ILoggerFactory loggerFactory) : IProcessorContextFactory
{
    public Result<ProcessorContext> Create(
        Type processorType,
        JobRun jobRun,
        JobStepName jobStepName)
    {
        var jobRunStep = jobRun.Steps
            .SingleOrDefault(ja => ja.StepName.Equals(jobStepName));

        if (jobRunStep is null)
        {
            return JobErrors.StepNotFound(jobStepName);
        }

        var properties = jobRunStep.StepProperties
            .Select(p => new ProcessorProperty(p.Name, p.Value))
            .ToList();

        var processorConfiguration = processorProvider.ResolveConfiguration(jobRunStep.ProcessorName);
        if (processorConfiguration.IsFailure)
        {
            return processorConfiguration.Error;
        }

        properties.AddRange(
            processorConfiguration.Value.Settings.Select(p => new ProcessorProperty(p.Key, p.Value))
        );

        var processorInputs = new List<ProcessorInput>();
        foreach (var input in jobRunStep.StepInputs)
        {
            var resolveAsset = jobRun.ResolveAsset(input.AssetName);
            if (!resolveAsset.IsSome)
            {
                return JobAssetErrors.NotFound(input.AssetName);
            }

            var asset = resolveAsset.ValueOrThrow();
            if (!asset.IsAvailable)
            {
                return JobAssetErrors.NotAvailable(input.AssetName);
            }

            var processorInput = new ProcessorInput(input.Name, asset.Uri!, asset.MediaInformation!);
            processorInputs.Add(processorInput);
        }

        var processorOutputs = new List<ProcessorOutput>();
        foreach (var output in jobRunStep.StepOutputs)
        {
            var resolveAsset = jobRun.ResolveAsset(output.AssetName, JobAssetKind.Output);

            if (resolveAsset.IsSome)
            {
                var jobOutput = new ProcessorOutput(
                    name: output.Name,
                    assetName: output.AssetName.ToString(),
                    uri: resolveAsset.ValueOrThrow().Uri!);

                processorOutputs.Add(jobOutput);
                continue;
            }

            var tempPath = Path.Combine(
                path1: AppDomain.CurrentDomain.BaseDirectory,
                path2: "temp",
                path3: jobRun.JobId.ToString());

            Directory.CreateDirectory(tempPath);

            var processorOutput = new ProcessorOutput(
                name: output.Name,
                assetName: output.AssetName.ToString(),
                uri: Path.Combine(tempPath, output.AssetName.ToString()));

            processorOutputs.Add(processorOutput);
        }

        var context = ProcessorContext.Create(
            logger: loggerFactory.CreateLogger(processorType),
            pluginPath: processorConfiguration.Value.PluginPath,
            inputs: processorInputs,
            outputs: processorOutputs,
            properties: properties);
        return Result.Created(context);
    }
}
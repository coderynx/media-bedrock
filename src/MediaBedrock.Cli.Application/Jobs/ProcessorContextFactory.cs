using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.JobsStateMachine;
using MediaBedrock.Cli.Domain.Processors.Interfaces;
using MediaBedrock.Sdk.Processors;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Cli.Application.Jobs;

public sealed class ProcessorContextFactory(
    IProcessorProvider processorProvider,
    ILoggerFactory loggerFactory) : IProcessorContextFactory
{
    public Result<ProcessorContext> Create(Type processorType, JobStateMachine jobStateMachine, JobStepName jobStepName)
    {
        var stepStateMachine = jobStateMachine.StepsStateMachines
            .SingleOrDefault(ja => ja.StepName.Equals(jobStepName));

        if (stepStateMachine is null)
        {
            return JobErrors.StepNotFound(jobStepName);
        }

        var properties = stepStateMachine.StepProperties
            .Select(p => new ProcessorProperty(p.Name, p.Value))
            .ToList();

        var processorConfiguration = processorProvider.ResolveConfiguration(stepStateMachine.ProcessorName);
        if (processorConfiguration.IsSuccess)
        {
            properties.AddRange(
                processorConfiguration.Value.Settings.Select(p => new ProcessorProperty(p.Key, p.Value))
            );
        }

        var processorInputs = new List<ProcessorInput>();
        foreach (var input in stepStateMachine.StepInputs)
        {
            var resolveAsset = jobStateMachine.ResolveAsset(input.AssetName);
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
        foreach (var output in stepStateMachine.StepOutputs)
        {
            var resolveAsset = jobStateMachine.ResolveAsset(output.AssetName, JobAssetKind.Output);

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
                path3: jobStateMachine.Job.Id.ToString());

            Directory.CreateDirectory(tempPath);

            var processorOutput = new ProcessorOutput(
                name: output.Name,
                assetName: output.AssetName.ToString(),
                uri: Path.Combine(tempPath, output.AssetName.ToString()));

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
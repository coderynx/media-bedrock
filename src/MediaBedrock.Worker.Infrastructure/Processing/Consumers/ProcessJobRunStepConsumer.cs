using System.Text.Json;
using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Worker.Application.Processing.Interfaces;
using MediaBedrock.Worker.Domain.Processing.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Worker.Infrastructure.Processing.Consumers;

public sealed class ProcessJobRunStepConsumer(
    IProcessorInstancesService processorInstancesService,
    ILogger<ProcessJobRunStepConsumer> logger)
    : IConsumer<JobRunStepStartedIntegrationEvent>
{
    public async Task ConsumeAsync(
        ConsumerContext<JobRunStepStartedIntegrationEvent> context,
        CancellationToken ct = new())
    {
        var message = context.Message;

        var createProcessorName = ProcessorName.Create(message.ProcessorName);
        if (createProcessorName.IsFailure)
        {
            logger.LogError("Invalid processor name: {ProcessorName}", message.ProcessorName);
            return;
        }

        var processorName = createProcessorName.Value;

        var processorInputs = new List<ProcessorInstanceInput>();
        foreach (var i in message.Inputs)
        {
            var assetInformation = JsonSerializer.Deserialize<AssetInformation>(i.MediaInformation);
            if (assetInformation is not null)
            {
                var createProcessorInstanceInput = ProcessorInstanceInput.Create(i.Name, i.Uri, assetInformation);
                if (createProcessorInstanceInput.IsFailure)
                {
                    logger.LogError("Failed to create processor instance input for input {InputName}", i.Name);
                    continue;
                }

                processorInputs.Add(createProcessorInstanceInput.Value);
                continue;
            }

            logger.LogError("Failed to deserialize asset information for input {InputName}", i.Name);
        }

        var processorOutputs = new List<ProcessorInstanceOutput>();
        foreach (var o in message.Outputs)
        {
            var createProcessorInstanceOutput = ProcessorInstanceOutput.Create(o.Name, o.Uri);
            if (createProcessorInstanceOutput.IsFailure)
            {
                logger.LogError("Failed to create processor instance output for output {OutputName}", o.Name);
                continue;
            }

            processorOutputs.Add(createProcessorInstanceOutput.Value);
        }

        var processorProperties = new List<ProcessorInstanceProperty>();
        foreach (var p in message.Properties)
        {
            var createProcessorInstanceProperty = ProcessorInstanceProperty.Create(p.Name, p.Value);
            if (createProcessorInstanceProperty.IsFailure)
            {
                logger.LogError("Failed to create processor instance property for property {PropertyName}", p.Name);
                continue;
            }

            processorProperties.Add(createProcessorInstanceProperty.Value);
        }

        var createJobRunStepId = JobRunStepId.Create(message.JobRunStepId);
        if (createJobRunStepId.IsFailure)
        {
            logger.LogError("Invalid job run step id: {JobRunStepId}", message.JobRunStepId);
            return;
        }

        var jobRunStepId = createJobRunStepId.Value;

        var createProcessor = await processorInstancesService.CreateAsync(
            jobRunStepId: jobRunStepId,
            processorName: processorName,
            inputs: processorInputs,
            outputs: processorOutputs,
            properties: processorProperties,
            cancellationToken: ct);

        if (createProcessor.IsFailure)
        {
            logger.LogError("Failed to create processor {ProcessorName}", message.ProcessorName);
        }

        var processorInstanceId = createProcessor.Value.Id;

        var startProcessor = await processorInstancesService.StartAsync(processorInstanceId, ct);
        if (startProcessor.IsFailure)
        {
            logger.LogError("Failed to start processor instance {ProcessorInstanceId}", processorInstanceId);
        }
    }
}
using System.Text.Json;
using Coderynx.MessagingKit.Abstractions;
using MediaBedrock.Contracts.JobRuns;
using MediaBedrock.Controller.Application.JobRuns.Interfaces;
using MediaBedrock.Controller.Domain.JobRuns.DomainEvents;
using MediaBedrock.Core.Domain.Abstractions;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Controller.Application.JobRuns.DomainEventConsumers;

public sealed class JobRunStepStartedDomainEventConsumer(
    IJobRunStepsService jobRunStepsService,
    IStorageService storageService,
    ILogger<JobRunStepStartedDomainEventConsumer> logger,
    IMessagePublisher messagePublisher)
    : IDomainEventConsumer<JobRunStepStartedDomainEvent>
{
    public async Task ConsumeAsync(
        JobRunStepStartedDomainEvent domainEvent,
        CancellationToken cancellationToken = new())
    {
        var getJobRunStep = await jobRunStepsService.GetAsync(domainEvent.JobRunStepId, cancellationToken);
        if (getJobRunStep.IsFailure)
        {
            logger.LogError("Failed to get job run step {JobRunStepId}", domainEvent.JobRunStepId);
            return;
        }

        var jobRunStep = getJobRunStep.Value;


        var inputs = new List<JobRunStepStartedIntegrationEvent.Input>();
        foreach (var stepInput in jobRunStep.StepInputs)
        {
            var getAsset = await storageService.GetAssetAsync(stepInput.AssetName, cancellationToken);
            if (getAsset.IsFailure)
            {
                logger.LogError("Failed to get asset {AssetName}", stepInput.AssetName);
                return;           
            }
            
            var asset = getAsset.Value;
            
            var createAssetUri = await storageService.CreateAssetUriAsync(asset.Name, cancellationToken);
            if (createAssetUri.IsFailure)
            {
                logger.LogError("Failed to create asset uri for asset {AssetName}", stepInput.AssetName);
                return;           
            }
            
            var assetUri = createAssetUri.Value;
            
            var input = new JobRunStepStartedIntegrationEvent.Input(
                Name: stepInput.Name,
                Uri: assetUri,
                MediaInformation: JsonSerializer.Serialize(asset.MediaInformation));
            
            inputs.Add(input);
        }

        var outputs = new List<JobRunStepStartedIntegrationEvent.Output>();
        foreach (var stepOutput in jobRunStep.StepOutputs)
        {
            var getAsset = await storageService.GetAssetAsync(stepOutput.AssetName, cancellationToken);
            if (getAsset.IsFailure)
            {
                logger.LogError("Failed to get asset {AssetName}", stepOutput.AssetName);
                return;           
            }
            
            var asset = getAsset.Value;
            
            var createAssetUri = await storageService.CreateAssetUriAsync(asset.Name, cancellationToken);
            if (createAssetUri.IsFailure)
            {
                logger.LogError("Failed to create asset uri for asset {AssetName}", stepOutput.AssetName);
                return;           
            }
            
            var assetUri = createAssetUri.Value;
            
            var output = new JobRunStepStartedIntegrationEvent.Output(Name: stepOutput.Name, Uri: assetUri);
            
            outputs.Add(output);
        }

        var properties = jobRunStep.StepProperties
            .Select(property => new JobRunStepStartedIntegrationEvent.Property(property.Name, property.Value))
            .ToList();

        var integrationEvent = new JobRunStepStartedIntegrationEvent(
            JobRunStepId: jobRunStep.Id.Value,
            ProcessorName: jobRunStep.ProcessorName.ToString(),
            Inputs: inputs,
            Outputs: outputs,
            Properties: properties);

        await messagePublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
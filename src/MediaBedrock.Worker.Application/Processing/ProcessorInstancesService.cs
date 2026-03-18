using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Worker.Application.Database;
using MediaBedrock.Worker.Application.Processing.Interfaces;
using MediaBedrock.Worker.Domain.Processing;
using MediaBedrock.Worker.Domain.Processing.Entities;
using MediaBedrock.Worker.Domain.Processing.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Worker.Application.Processing;

public sealed class ProcessorInstancesService(IWorkerDbContext dbContext, ILogger<ProcessorInstancesService> logger)
    : IProcessorInstancesService
{
    public async Task<Result<ProcessorInstance>> CreateAsync(
        JobRunStepId jobRunStepId,
        ProcessorName processorName,
        IEnumerable<ProcessorInstanceInput> inputs,
        IEnumerable<ProcessorInstanceOutput> outputs,
        IEnumerable<ProcessorInstanceProperty> properties,
        CancellationToken cancellationToken = new())
    {
        var instance = ProcessorInstance.Create(
            jobRunStepId: jobRunStepId,
            processorName: processorName,
            inputs: inputs,
            outputs: outputs,
            properties: properties);

        await dbContext.ProcessorInstances.AddAsync(instance, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Started processor instance {ProcessorInstanceId}", instance.Id);

        return Result.Created(instance);
    }

    public async Task<Result> StartAsync(
        ProcessorInstanceId processorInstanceId,
        CancellationToken cancellationToken = new())
    {
        var instance = await dbContext.ProcessorInstances
            .SingleOrDefaultAsync(pi => pi.Id.Equals(processorInstanceId), cancellationToken);

        if (instance is null)
        {
            return ProcessorInstanceErrors.NotFound(processorInstanceId);
        }

        var result = instance.Start();
        if (result.IsFailure)
        {
            logger.LogError(
                "Failed to start processor instance {ProcessorInstanceId}: {Error}",
                processorInstanceId,
                result.Error);

            return result.Error;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Updated();
    }

    public async Task<Option<ProcessorInstance>> GetAsync(
        ProcessorInstanceId processorInstanceId,
        CancellationToken cancellationToken = new())
    {
        var processorInstance = await dbContext.ProcessorInstances
            .AsNoTracking()
            .SingleOrDefaultAsync(pi => pi.Id.Equals(processorInstanceId), cancellationToken);

        return processorInstance is null 
            ? Option.None<ProcessorInstance>() 
            : Option.Some(processorInstance);
    }
}
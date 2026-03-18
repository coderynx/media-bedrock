using Coderynx.Functional.Results;
using Coderynx.Functional.Results.Successes;
using MediaBedrock.Worker.Application.Database;
using MediaBedrock.Worker.Application.Processing.Interfaces;
using MediaBedrock.Worker.Domain.Processing;
using MediaBedrock.Worker.Domain.Processing.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace MediaBedrock.Worker.Application.Processing;

public sealed class ProcessorRunner(
    IWorkerDbContext dbContext,
    IProcessorProvider processorProvider,
    IProcessorContextFactory processorContextFactory,
    ILogger<ProcessorRunner> logger) : IProcessorRunner
{
    public async Task<Result> RunAsync(
        ProcessorInstanceId processorInstanceId,
        CancellationToken cancellationToken = new())
    {
        var processorInstance = await dbContext.ProcessorInstances
            .SingleOrDefaultAsync(pi => pi.Id.Equals(processorInstanceId), cancellationToken);

        if (processorInstance is null)
        {
            return ProcessorInstanceErrors.NotFound(processorInstanceId);
        }

        const string processorInstanceIdPropertyName = "ProcessorInstanceId";
        const string processorNamePropertyName = "ProcessorName";

        var resolveProcessor = processorProvider.ResolveProcessor(processorInstance.ProcessorName);
        if (resolveProcessor.IsFailure)
        {
            logger.LogError("Processor {ProcessorName} not found", processorInstance.ProcessorName);
            return resolveProcessor.Error;
        }

        var processor = resolveProcessor.Value;

        var createContext = processorContextFactory.Create(processor.GetType(), processorInstance);

        if (createContext.IsFailure)
        {
            logger.LogError("Failed to create processor context {Error}", createContext.Error.Message);
            return createContext.Error;
        }

        using (LogContext.PushProperty(processorInstanceIdPropertyName, processorInstance.Id))
        using (LogContext.PushProperty(processorNamePropertyName, processorInstance.ProcessorName))
        {
            var processorResult = await Result.TryCatchAsync<Result>(
                onTry: async () =>
                {
                    var processResult = await processor.ProcessAsync(
                        context: createContext.Value,
                        cancellationToken: cancellationToken);

                    return processResult.IsSuccess
                        ? Success.Created(processResult)
                        : ProcessorInstanceErrors.RunFailed(processResult.Message);
                },
                onCatch: exception => ProcessorInstanceErrors.RunFailed(exception.Message));

            if (processorResult.IsFailure)
            {
                processorInstance.Fail();
                return processorResult.Error;
            }

            processorInstance.Complete();
        }

        return Result.Updated();
    }
}
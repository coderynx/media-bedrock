using MediaBedrock.Cli.Application.Jobs;
using MediaBedrock.Cli.Application.Jobs.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediaBedrock.Cli.Infrastructure.Jobs;

internal sealed class JobEventProcessorBackgroundService(
    IServiceScopeFactory serviceScopeFactory,
    JobMessageQueue queue,
    IJobStateMachineRepository jobStateMachineRepository,
    ILogger<JobEventProcessorBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in queue.Reader.ReadAllAsync(stoppingToken))
        {
            using var scope = serviceScopeFactory.CreateScope();

            var jobWorkflow = jobStateMachineRepository.Get(message.JobId);
            if (!jobWorkflow.IsSome)
            {
                logger.LogError("Failed to get job workflow for {JobId}", message.JobId);
                continue;
            }

            // TODO: Use source generators for resolving handlers.

            var context = ActivatorUtilities.CreateInstance(
                scope.ServiceProvider,
                typeof(JobMessageContext<>).MakeGenericType(message.GetType()),
                jobWorkflow.ValueOrThrow(),
                message);

            var handlerType = typeof(IJobMessageHandler<>).MakeGenericType(message.GetType());
            var handler = scope.ServiceProvider.GetService(handlerType);

            if (handler is null)
            {
                logger.LogError("Failed to resolve handler for message {MessageType}", message.GetType());
                continue;
            }

            var handleMethod = handlerType.GetMethod("HandleAsync");
            if (handleMethod is null)
            {
                logger.LogError("Failed to resolve handle method for handler {HandlerType}", handlerType);
                continue;
            }

            if (handleMethod.Invoke(handler, [context, stoppingToken]) is Task task)
            {
                // TODO: Find a better way to handle this.
                _ = Task.Run(async () => await task, stoppingToken);
            }
        }
    }
}
using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Worker.Domain.Processing.Entities;
using MediaBedrock.Worker.Domain.Processing.ValueObjects;

namespace MediaBedrock.Worker.Application.Processing.Interfaces;

public interface IProcessorInstancesService
{
    Task<Result<ProcessorInstance>> CreateAsync(
        JobRunStepId jobRunStepId,
        ProcessorName processorName,
        IEnumerable<ProcessorInstanceInput> inputs,
        IEnumerable<ProcessorInstanceOutput> outputs,
        IEnumerable<ProcessorInstanceProperty> properties,
        CancellationToken cancellationToken = new());

    Task<Result> StartAsync(
        ProcessorInstanceId processorInstanceId,
        CancellationToken cancellationToken = new());

    Task<Option<ProcessorInstance>> GetAsync(
        ProcessorInstanceId processorInstanceId,
        CancellationToken cancellationToken);
}
using Coderynx.Functional.Results;
using MediaBedrock.Worker.Domain.Processing.ValueObjects;

namespace MediaBedrock.Worker.Application.Processing.Interfaces;

public interface IProcessorRunner
{
    Task<Result> RunAsync(
        ProcessorInstanceId processorInstanceId,
        CancellationToken cancellationToken = new());
}
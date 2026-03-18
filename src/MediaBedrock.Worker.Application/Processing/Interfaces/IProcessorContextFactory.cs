using Coderynx.Functional.Results;
using MediaBedrock.Worker.Domain.Processing.Entities;
using MediaBedrock.Worker.Sdk.Processors;

namespace MediaBedrock.Worker.Application.Processing.Interfaces;

/// <summary>
///     Factory interface for creating instances of <see cref="ProcessorContext" />.
/// </summary>
public interface IProcessorContextFactory
{
    Result<ProcessorContext> Create(Type processorType, ProcessorInstance processorInstance);
}
using Coderynx.Functional.Results;
using MediaBedrock.Domain.Jobs.Steps;
using MediaBedrock.Domain.JobStateMachines;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Domain.Processors.Interfaces;

/// <summary>
///     Factory interface for creating instances of <see cref="ProcessorContext" />.
/// </summary>
public interface IProcessorContextFactory
{
    Result<ProcessorContext> Create(
        Type processorType,
        JobStateMachine jobStateMachine,
        JobStepName jobStepName);
}
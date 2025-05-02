using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Cli.Domain.Processors.Interfaces;

/// <summary>
///     Factory interface for creating instances of <see cref="ProcessorContext" />.
/// </summary>
public interface IProcessorContextFactory
{
    Result<ProcessorContext> Create(Type processorType, JobStateMachine jobStateMachine, JobStepName jobStepName);
}
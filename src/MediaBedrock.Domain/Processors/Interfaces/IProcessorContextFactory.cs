using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobRuns;
using MediaBedrock.Domain.Jobs.Steps;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Domain.Processors.Interfaces;

/// <summary>
///     Factory interface for creating instances of <see cref="ProcessorContext" />.
/// </summary>
public interface IProcessorContextFactory
{
    Result<ProcessorContext> Create(
        Type processorType,
        JobRun jobRun,
        JobStepName jobStepName);
}
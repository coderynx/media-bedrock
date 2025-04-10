using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Processors;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IProcessorProvider
{
    Result<IProcessor> ResolveProcessor(ProcessorName name);
    Result<ProcessorConfiguration> ResolveConfiguration(ProcessorName name);
}
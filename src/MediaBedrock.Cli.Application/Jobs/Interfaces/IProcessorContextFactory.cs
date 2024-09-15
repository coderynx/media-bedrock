using Coderynx.Functional.Result;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IProcessorContextFactory
{
    Result<ProcessorContext> Create(JobStep step, JobAssetsPool assetsPool);
}
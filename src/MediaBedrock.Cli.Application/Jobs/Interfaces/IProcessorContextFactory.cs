using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Assets;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Cli.Application.Jobs.Interfaces;

public interface IProcessorContextFactory
{
    Result<ProcessorContext> Create(JobId jobId, JobStep step, JobAssetsPool assetsPool);
}
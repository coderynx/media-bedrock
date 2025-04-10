using MediaBedrock.Cli.Domain.Jobs.Assets;
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Cli.Domain.Jobs;

public sealed class JobContainer(JobId id, Dictionary<JobStep, IProcessor> processors, JobAssetsPool assetsPool)
{
    public JobId Id { get; init; } = id;
    public Dictionary<JobStep, IProcessor> Processors { get; init; } = processors;
    public JobAssetsPool AssetsPool { get; init; } = assetsPool;
}
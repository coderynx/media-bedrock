using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.JobAssets;

namespace MediaBedrock.Cli.Domain.Jobs.Steps;

public sealed record JobStepSource
{
    private JobStepSource()
    {
    }

    public required string Name { get; init; }
    public required JobAssetName AssetName { get; init; }

    public static Result<JobStepSource> Create(string name, JobAssetName assetName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return JobErrors.InvalidSourceName(name);
        }

        var jonStepSource = new JobStepSource
        {
            Name = name,
            AssetName = assetName
        };

        return Result.Created(jonStepSource);
    }
}
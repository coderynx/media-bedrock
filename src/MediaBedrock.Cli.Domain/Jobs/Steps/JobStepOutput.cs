using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.JobAssets;

namespace MediaBedrock.Cli.Domain.Jobs.Steps;

public sealed record JobStepOutput
{
    private JobStepOutput()
    {
    }

    public required string Name { get; init; }
    public required JobAssetName AssetName { get; init; }

    public static Result<JobStepOutput> Create(string name, JobAssetName assetName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return JobErrors.InvalidOutputName(name);
        }

        var jonStepSource = new JobStepOutput
        {
            Name = name,
            AssetName = assetName
        };

        return Result.Created(jonStepSource);
    }
}
using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobAssets;

namespace MediaBedrock.Domain.Jobs.Steps;

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

        var jobStepSource = new JobStepOutput
        {
            Name = name,
            AssetName = assetName
        };

        return Result.Created(jobStepSource);
    }
}
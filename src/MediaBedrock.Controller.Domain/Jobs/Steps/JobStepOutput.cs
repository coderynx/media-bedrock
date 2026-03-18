using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.JobAssets;

namespace MediaBedrock.Controller.Domain.Jobs.Steps;

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
using Coderynx.Functional.Results;
using MediaBedrock.Domain.JobAssets;

namespace MediaBedrock.Domain.Jobs.Steps;

public sealed record JobStepInput
{
    private JobStepInput()
    {
    }

    public required string Name { get; init; }
    public required JobAssetName AssetName { get; init; }

    public static Result<JobStepInput> Create(string name, JobAssetName assetName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return JobErrors.InvalidInputName(name);
        }

        var stepInput = new JobStepInput
        {
            Name = name,
            AssetName = assetName
        };

        return Result.Created(stepInput);
    }
}
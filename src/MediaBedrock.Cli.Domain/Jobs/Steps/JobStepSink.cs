using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.JobAssets;

namespace MediaBedrock.Cli.Domain.Jobs.Steps;

public sealed record JobStepSink
{
    private JobStepSink()
    {
    }

    public required string Name { get; init; }
    public required JobAssetName AssetName { get; init; }

    public static Result<JobStepSink> Create(string name, JobAssetName assetName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return JobErrors.InvalidSinkName(name);
        }

        var jobStepSink = new JobStepSink
        {
            Name = name,
            AssetName = assetName
        };

        return Result.Created(jobStepSink);
    }
}
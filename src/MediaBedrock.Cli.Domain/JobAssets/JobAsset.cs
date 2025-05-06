using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.JobsStateMachine;
using MediaBedrock.Sdk.Processors;

namespace MediaBedrock.Cli.Domain.JobAssets;

public sealed class JobAsset
{
    private JobAsset()
    {
    }
    
    public required JobAssetId Id { get; init; }
    public required JobStateMachine JobStateMachine { get; init; }
    public required JobAssetName Name { get; init; }
    public string? Uri { get; private set; }
    public JobAssetKind Kind { get; private init; }
    public MediaInformation? MediaInformation { get; set; }
    public bool IsAvailable => !string.IsNullOrWhiteSpace(Uri);

    public void UpdateUri(string? uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            return;
        }

        Uri = uri;
    }

    public static Result<JobAsset> CreateInput(
        JobStateMachine jobStateMachine,
        JobAssetName name,
        string uri,
        MediaInformation mediaInformation)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            return JobAssetErrors.InvalidUri(uri);
        }

        var jobAsset = new JobAsset
        {
            Id = new JobAssetId(),
            JobStateMachine = jobStateMachine,
            Name = name,
            Uri = uri,
            Kind = JobAssetKind.Input,
            MediaInformation = mediaInformation
        };

        return Result.Created(jobAsset);
    }

    public static Result<JobAsset> CreateOutput(JobStateMachine jobStateMachine, JobAssetName name, string uri)
    {
        var jobAsset = new JobAsset
        {
            Id = new JobAssetId(),
            JobStateMachine = jobStateMachine,
            Name = name,
            Uri = uri,
            Kind = JobAssetKind.Output
        };

        return Result.Created(jobAsset);
    }

    public static Result<JobAsset> CreateMezzanine(JobStateMachine jobStateMachine, JobAssetName name)
    {
        var jobAsset = new JobAsset
        {
            JobStateMachine = jobStateMachine,
            Id = new JobAssetId(),
            Name = name,
            Kind = JobAssetKind.Mezzanine
        };

        return Result.Created(jobAsset);
    }
}
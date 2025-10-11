using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Domain.Abstractions;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.JobRuns.DomainEvents;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Steps;

namespace MediaBedrock.Domain.JobRuns;

public sealed class JobRun : Entity
{
    private readonly List<JobAsset> _assetsPool = [];
    private readonly List<JobRunStep> _steps = [];

    private JobRun()
    {
    }

    public required JobRunId Id { get; init; }
    public required JobId JobId { get; init; }
    public JobRunStatus Status { get; private set; } = JobRunStatus.Pending;
    public JobRunError? Error { get; private set; }
    public IReadOnlyList<JobRunStep> Steps => _steps;
    public IReadOnlyList<JobAsset> AssetsPool => _assetsPool;

    public static JobRun Create(JobId jobId)
    {
        var jobRun = new JobRun
        {
            Id = JobRunId.Create(),
            JobId = jobId
        };

        return jobRun;
    }

    public Result Start()
    {
        if (Status is not JobRunStatus.Pending)
        {
            return JobRunErrors.InvalidStatusTransition(Status, JobRunStatus.Running);
        }

        Status = JobRunStatus.Running;

        var startedEvent = new JobRunStartedDomainEvent(Id);
        Raise(startedEvent);

        return Result.Updated();
    }

    public void TransitionToFailed(JobFailureReason failureReason, string message)
    {
        Error = new JobRunError(failureReason, message);
        Status = JobRunStatus.Failed;
    }

    public void TransitionToCompleted()
    {
        Status = JobRunStatus.Completed;
    }

    public void AddStep(JobStep jobStep)
    {
        var jobRunStep = JobRunStep.Create(this, jobStep);
        _steps.AddRange(jobRunStep);
    }

    public void AddAsset(JobAsset asset)
    {
        if (_assetsPool.Any(a => a.Name.Equals(asset.Name)))
        {
            return;
        }

        _assetsPool.Add(asset);
    }

    public List<JobAsset> ResolveAssets(JobAssetKind kind)
    {
        return _assetsPool.Where(a => a.Kind.Equals(kind)).ToList();
    }

    public Option<JobAsset> ResolveAsset(JobAssetName name)
    {
        var asset = _assetsPool.FirstOrDefault(a => a.Name.Equals(name));

        return asset is null
            ? Option.None<JobAsset>()
            : Option.Some(asset);
    }

    public Option<JobAsset> ResolveAsset(JobAssetName name, JobAssetKind kind)
    {
        var asset = _assetsPool.FirstOrDefault(a => a.Name.Equals(name) && a.Kind == kind);

        return asset is null
            ? Option.None<JobAsset>()
            : Option.Some(asset);
    }

    public bool DoesAssetExist(JobAssetName name)
    {
        return _assetsPool.Any(a => a.Name.Equals(name));
    }
}
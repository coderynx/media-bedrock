using Coderynx.Functional.Options;
using MediaBedrock.Domain.JobAssets;
using MediaBedrock.Domain.Jobs;
using MediaBedrock.Domain.Jobs.Steps;

namespace MediaBedrock.Domain.JobStateMachine;

public sealed class JobStateMachine
{
    private readonly List<JobStateMachineTag> _tags = [];
    private readonly List<JobAsset> _assetsPool = [];
    private readonly List<JobStepStateMachine> _stepStateMachines = [];

    private JobStateMachine()
    {
    }

    public required JobStateMachineId Id { get; init; }
    public required Job Job { get; init; }
    public JobExecutionStatus ExecutionStatus { get; private set; } = JobExecutionStatus.Pending;
    public JobExecutionError? ExecutionError { get; private set; }
    public IReadOnlyCollection<JobStateMachineTag> Tags => _tags;
    public IReadOnlyList<JobStepStateMachine> StepStateMachines => _stepStateMachines;
    public IReadOnlyList<JobAsset> AssetsPool => _assetsPool;

    public static JobStateMachine Create(Job job)
    {
        var stateMachine = new JobStateMachine
        {
            Id = JobStateMachineId.Create(),
            Job = job
        };

        return stateMachine;
    }

    public void Tag(JobStateMachineTag tag)
    {
        if (_tags.Any(t => t.Equals(tag)))
        {
            return;
        }

        _tags.Add(tag);
    }

    public void Untag(JobStateMachineTag tag)
    {
        if (!_tags.Any(t => t.Equals(tag)))
        {
            return;
        }

        _tags.Remove(tag);
    }

    public void TransitionToRunning()
    {
        ExecutionStatus = JobExecutionStatus.Running;
    }

    public void TransitionToFailed(JobFailureReason failureReason, string message)
    {
        ExecutionError = new JobExecutionError(failureReason, message);
        ExecutionStatus = JobExecutionStatus.Failed;
    }

    public void TransitionToCompleted()
    {
        ExecutionStatus = JobExecutionStatus.Completed;
    }

    public void AddStep(JobStep jobStep)
    {
        var jobStepStateMachine = JobStepStateMachine.Create(this, jobStep);
        _stepStateMachines.AddRange(jobStepStateMachine);
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
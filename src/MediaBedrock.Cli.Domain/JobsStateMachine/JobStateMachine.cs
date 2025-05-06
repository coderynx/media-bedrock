using Coderynx.Functional.Options;
using MediaBedrock.Cli.Domain.JobAssets;
using MediaBedrock.Cli.Domain.Jobs;
using MediaBedrock.Cli.Domain.Jobs.Steps;

namespace MediaBedrock.Cli.Domain.JobsStateMachine;

public sealed class JobStateMachine
{
    private readonly List<JobAsset> _assetsPool = [];
    private readonly List<JobStepStateMachine> _stepsStateMachines = [];

    private JobStateMachine()
    {
    }

    public required JobStateMachineId Id { get; init; }
    public required Job Job { get; init; }
    public JobExecutionStatus ExecutionStatus { get; private set; } = JobExecutionStatus.Pending;
    public IReadOnlyList<JobStepStateMachine> StepsStateMachines => _stepsStateMachines;
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

    public void TransitionToRunning()
    {
        ExecutionStatus = JobExecutionStatus.Running;
    }

    public void TransitionToCompleted()
    {
        ExecutionStatus = JobExecutionStatus.Completed;
    }

    public void TransitionToFailed()
    {
        ExecutionStatus = JobExecutionStatus.Failed;
    }

    public void AddStep(JobStep jobStep)
    {
        var jobStepStateMachine = JobStepStateMachine.Create(this, jobStep);
        _stepsStateMachines.AddRange(jobStepStateMachine);
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
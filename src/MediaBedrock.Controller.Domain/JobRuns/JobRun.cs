using Coderynx.Functional.Options;
using Coderynx.Functional.Results;
using MediaBedrock.Controller.Domain.JobAssets;
using MediaBedrock.Controller.Domain.JobRuns.DomainEvents;
using MediaBedrock.Controller.Domain.Jobs;
using MediaBedrock.Controller.Domain.Jobs.Steps;
using MediaBedrock.Core.Domain.Abstractions;

namespace MediaBedrock.Controller.Domain.JobRuns;

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

    /// <summary>
    /// Initiates the start process for the job run by transitioning its status from 'Pending' to 'Running'.
    /// This method also raises a domain event to indicate the job run has started.
    /// </summary>
    /// <returns>
    /// A result indicating the outcome of the operation. Returns a failure result if the current status is not
    /// 'Pending' or an updated result when the status is successfully transitioned to 'Running'.
    /// </returns>
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

    /// <summary>
    /// Marks the job run as completed by transitioning its status from 'Running' to 'Completed'.
    /// Returns an error if the current status is not 'Running'.
    /// </summary>
    /// <returns>
    /// A result indicating the outcome of the operation. Returns a failure result if the status
    /// transition is invalid, or an updated result when the transition is successfully completed.
    /// </returns>
    public Result Complete()
    {
        if (Status is not JobRunStatus.Running)
        {
            return JobRunErrors.InvalidStatusTransition(Status, JobRunStatus.Completed);
        }
        
        Status = JobRunStatus.Completed;
        
        return Result.Updated();
    }

    public Result Fail(JobFailureReason failureReason, string message)
    {
        if (Status is not JobRunStatus.Running)
        {
            return JobRunErrors.InvalidStatusTransition(Status, JobRunStatus.Failed);
        }

        Error = new JobRunError(failureReason, message);
        Status = JobRunStatus.Failed;

        return Result.Updated();
    }

    public Result Advance()
    {
        if (Status is not JobRunStatus.Running)
        {
            return JobRunErrors.InvalidStatusTransition(Status, JobRunStatus.Running);
        }
        
        var areStepsCompleted = !Steps.Any(ja => ja.Status is JobRunStepStatus.Running or JobRunStepStatus.Pending);
        if (areStepsCompleted)
        {
            var complete = Complete();
            
            return complete.IsFailure 
                ? complete.Error 
                : Result.Updated();
        }
        
        foreach (var step in Steps)
        {
            var startStep = step.Start();

            switch (startStep.IsFailure)
            {
                case true when startStep.Error.Code is JobRunErrorCodes.StepNotReadyToRun:
                    continue;
                case true:
                    return startStep.Error;
            }
        }

        return Result.Updated();
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

    public Option<JobAsset> ResolveAsset(JobAssetName name)
    {
        var asset = _assetsPool.FirstOrDefault(a => a.Name.Equals(name));

        return asset is null
            ? Option.None<JobAsset>()
            : Option.Some(asset);
    }

    public bool DoesAssetExist(JobAssetName name)
    {
        return _assetsPool.Any(a => a.Name.Equals(name));
    }
}
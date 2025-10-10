using Coderynx.Functional.Results;
using MediaBedrock.Domain.Jobs.Steps;
using MediaBedrock.Domain.Processors;

namespace MediaBedrock.Domain.JobRuns;

public sealed class JobRunStep
{
    private List<JobStepInput> _inputs = [];
    private List<JobStepOutput> _outputs = [];
    private List<JobStepProperty> _properties = [];

    private JobRunStep()
    {
    }

    public required JobRunStepId Id { get; init; }
    public required JobRun JobRun { get; init; }
    public required JobRunStepOrder Order { get; init; }
    public required JobStepName StepName { get; init; }
    public required ProcessorName ProcessorName { get; init; }
    public IReadOnlyList<JobStepInput> StepInputs => _inputs;
    public IReadOnlyList<JobStepOutput> StepOutputs => _outputs;
    public IReadOnlyList<JobStepProperty> StepProperties => _properties;
    public JobRunStepStatus Status { get; private set; }
    public JobRunStepError? ExecutionError { get; private set; }

    public static JobRunStep Create(JobRun jobRun, JobStep step)
    {
        return new JobRunStep
        {
            Id = new JobRunStepId(),
            JobRun = jobRun,
            Order = JobRunStepOrder.Create(step.Order),
            StepName = step.Name,
            ProcessorName = step.ProcessorName,
            _properties = step.Properties.ToList(),
            _inputs = step.Inputs.ToList(),
            _outputs = step.Outputs.ToList(),
            Status = JobRunStepStatus.Pending
        };
    }

    public Result TransitionToRunning()
    {
        if (Status is not JobRunStepStatus.Pending)
        {
            return JobRunErrors.InvalidStepStatusTransition(
                currentStatus: Status,
                targetStatus: JobRunStepStatus.Pending);
        }

        Status = JobRunStepStatus.Running;

        return Result.Updated();
    }

    public Result TransitionToCompleted()
    {
        if (Status is not JobRunStepStatus.Running)
        {
            return JobRunErrors.InvalidStepStatusTransition(
                currentStatus: Status,
                targetStatus: JobRunStepStatus.Running);
        }

        Status = JobRunStepStatus.Completed;

        return Result.Updated();
    }

    public Result TransitionToFailed(JobRunStepError error)
    {
        if (Status is not JobRunStepStatus.Running)
        {
            return JobRunErrors.InvalidStepStatusTransition(
                currentStatus: Status,
                targetStatus: JobRunStepStatus.Running);
        }

        Status = JobRunStepStatus.Failed;
        ExecutionError = error;

        return Result.Updated();
    }
}
using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.Processors;

namespace MediaBedrock.Cli.Domain.Jobs;

public sealed class JobStepStateMachine
{
    private List<JobStepProperty> _properties = new();
    private List<JobStepSink> _sinks = new();
    private List<JobStepSource> _sources = new();

    private JobStepStateMachine()
    {
    }

    public required JobStepStateMachineId Id { get; init; }
    public required JobStepName StepName { get; init; }
    public required ProcessorName ProcessorName { get; init; }
    public IReadOnlyList<JobStepSink> StepSinks => _sinks;
    public IReadOnlyList<JobStepSource> StepSources => _sources;
    public IReadOnlyList<JobStepProperty> StepProperties => _properties;
    public JobStepStatus Status { get; private set; }

    public static JobStepStateMachine Create(JobStep step)
    {
        return new JobStepStateMachine
        {
            Id = new JobStepStateMachineId(),
            StepName = step.Name,
            ProcessorName = step.ProcessorName,
            _properties = step.Properties.ToList(),
            _sinks = step.Sinks.ToList(),
            _sources = step.Sources.ToList(),
            Status = JobStepStatus.Pending
        };
    }

    public void TransitionToRunning()
    {
        Status = JobStepStatus.Running;
    }

    public void TransitionToCompleted()
    {
        Status = JobStepStatus.Completed;
    }

    public void TransitionToFailed()
    {
        Status = JobStepStatus.Failed;
    }
}
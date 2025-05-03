using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.Processors;

namespace MediaBedrock.Cli.Domain.JobsStateMachine;

public sealed class JobStepStateMachine
{
    private List<JobStepInput> _inputs = [];
    private List<JobStepOutput> _outputs = [];
    private List<JobStepProperty> _properties = [];

    private JobStepStateMachine()
    {
    }

    public required JobStepStateMachineId Id { get; init; }
    public required JobStateMachine JobStateMachine { get; init; }
    public required JobStepName StepName { get; init; }
    public required ProcessorName ProcessorName { get; init; }
    public IReadOnlyList<JobStepInput> StepInputs => _inputs;
    public IReadOnlyList<JobStepOutput> StepOutputs => _outputs;
    public IReadOnlyList<JobStepProperty> StepProperties => _properties;
    public JobStepExecutionStatus ExecutionStatus { get; private set; }
    public JobStepExecutionError? ExecutionError { get; private set; }

    public static JobStepStateMachine Create(JobStateMachine jobStateMachine, JobStep step)
    {
        return new JobStepStateMachine
        {
            Id = new JobStepStateMachineId(),
            JobStateMachine = jobStateMachine,
            StepName = step.Name,
            ProcessorName = step.ProcessorName,
            _properties = step.Properties.ToList(),
            _inputs = step.Inputs.ToList(),
            _outputs = step.Outputs.ToList(),
            ExecutionStatus = JobStepExecutionStatus.Pending
        };
    }

    public void TransitionToRunning()
    {
        ExecutionStatus = JobStepExecutionStatus.Running;
    }

    public void TransitionToCompleted()
    {
        ExecutionStatus = JobStepExecutionStatus.Completed;
    }

    public void TransitionToFailed(JobStepExecutionError executionError)
    {
        ExecutionStatus = JobStepExecutionStatus.Failed;
        ExecutionError = executionError;
    }
}
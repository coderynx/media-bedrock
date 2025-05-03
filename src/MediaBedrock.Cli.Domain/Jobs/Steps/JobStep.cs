using MediaBedrock.Cli.Domain.Processors;

namespace MediaBedrock.Cli.Domain.Jobs.Steps;

public sealed class JobStep
{
    private List<JobStepInput> _inputs = [];
    private List<JobStepOutput> _outputs = [];
    private List<JobStepProperty> _properties = [];

    private JobStep()
    {
    }

    public required JobStepId Id { get; init; }
    public required Job Job { get; init; }
    public required JobStepName Name { get; init; }
    public required ProcessorName ProcessorName { get; init; }
    public IReadOnlyList<JobStepProperty> Properties => _properties.AsReadOnly();
    public IReadOnlyList<JobStepInput> Inputs => _inputs.AsReadOnly();
    public IReadOnlyList<JobStepOutput> Outputs => _outputs.AsReadOnly();

    public static JobStep Create(
        Job job,
        JobStepName name,
        ProcessorName processorName,
        List<JobStepProperty> properties,
        List<JobStepInput> inputs,
        List<JobStepOutput> outputs)
    {
        return new JobStep
        {
            Id = JobStepId.Create(),
            Job = job,
            Name = name,
            ProcessorName = processorName,
            _properties = properties,
            _inputs = inputs,
            _outputs = outputs
        };
    }
}
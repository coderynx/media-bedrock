using MediaBedrock.Cli.Domain.Jobs.Processors;

namespace MediaBedrock.Cli.Domain.Jobs.Steps;

public sealed class JobStep
{
    public required JobStepName Name { get; init; }
    public required ProcessorName ProcessorName { get; init; }
    public List<JobStepProperty> Properties { get; init; } = [];
    public JobStepSink[] Sinks { get; init; } = [];
    public JobStepSource[] Sources { get; init; } = [];

    public static JobStep Create(
        JobStepName name,
        ProcessorName processorName,
        List<JobStepProperty> properties,
        JobStepSink[] inputs,
        JobStepSource[] outputs)
    {
        return new JobStep
        {
            Name = name,
            ProcessorName = processorName,
            Properties = properties,
            Sinks = inputs,
            Sources = outputs
        };
    }
}
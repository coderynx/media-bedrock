using MediaBedrock.Cli.Domain.Processors;

namespace MediaBedrock.Cli.Domain.Jobs.Steps;

public sealed class JobStep
{
    private List<JobStepProperty> _properties = [];
    private List<JobStepSink> _sinks = [];
    private List<JobStepSource> _sources = [];

    private JobStep()
    {
    }

    public required JobStepId Id { get; init; }
    public required Job Job { get; init; }
    public required JobStepName Name { get; init; }
    public required ProcessorName ProcessorName { get; init; }
    public IReadOnlyList<JobStepProperty> Properties => _properties.AsReadOnly();
    public IReadOnlyList<JobStepSink> Sinks => _sinks.AsReadOnly();
    public IReadOnlyList<JobStepSource> Sources => _sources.AsReadOnly();

    public static JobStep Create(
        Job job,
        JobStepName name,
        ProcessorName processorName,
        List<JobStepProperty> properties,
        List<JobStepSink> sinks,
        List<JobStepSource> sources)
    {
        return new JobStep
        {
            Id = JobStepId.Create(),
            Job = job,
            Name = name,
            ProcessorName = processorName,
            _properties = properties,
            _sinks = sinks,
            _sources = sources
        };
    }
}
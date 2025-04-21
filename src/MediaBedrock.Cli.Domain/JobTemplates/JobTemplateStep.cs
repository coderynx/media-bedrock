using MediaBedrock.Cli.Domain.Jobs.Processors;

namespace MediaBedrock.Cli.Domain.JobTemplates;

public sealed record JobTemplateStep
{
    public required string Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public required ProcessorName ProcessorName { get; init; }

    public IEnumerable<JobTemplateStepSink> Sinks { get; init; } = [];
    public IEnumerable<JobTemplateStepSource> Sources { get; init; } = [];
    public IEnumerable<JobTemplateStepProperty> Properties { get; init; } = [];
}
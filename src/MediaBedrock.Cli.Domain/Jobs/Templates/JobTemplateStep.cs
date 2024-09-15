namespace MediaBedrock.Cli.Domain.Jobs.Templates;

public sealed record JobStepTemplateSink
{
    public required string Name { get; init; }
    public required string Source { get; init; }
}

public sealed record JobStepTemplateSource
{
    public required string Name { get; init; }
    public required string Destination { get; init; }
}

public sealed record JobTemplateStep
{
    public required string Name { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public required string ProcessorName { get; init; }

    public JobStepTemplateSink[] Sinks { get; init; } = [];
    public JobStepTemplateSource[] Sources { get; init; } = [];
    public JobStepTemplateProperty[] Properties { get; init; } = [];
}
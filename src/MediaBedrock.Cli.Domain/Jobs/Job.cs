using MediaBedrock.Cli.Domain.Jobs.Steps;
using MediaBedrock.Cli.Domain.Jobs.Templates;

namespace MediaBedrock.Cli.Domain.Jobs;

public sealed class Job
{
    public required JobId Id { get; init; }
    public required JobTemplateName TemplateName { get; init; }
    public JobInput[] Inputs { get; init; } = [];
    public JobOutput[] Outputs { get; init; } = [];
    public JobStep[] Steps { get; init; } = [];

    public static Job Create(JobTemplateName templateName, JobInput[] inputs, JobOutput[] outputs, JobStep[] steps)
    {
        return new Job
        {
            Id = JobId.Create(),
            TemplateName = templateName,
            Inputs = inputs,
            Outputs = outputs,
            Steps = steps
        };
    }
}
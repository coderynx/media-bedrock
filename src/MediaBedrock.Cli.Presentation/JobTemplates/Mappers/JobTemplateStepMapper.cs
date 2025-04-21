using Coderynx.Functional.Results;
using MediaBedrock.Cli.Domain.Jobs.Processors;
using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

namespace MediaBedrock.Cli.Presentation.JobTemplates.Mappers;

internal static class JobTemplateStepMapper
{
    public static Result<JobTemplateStep> ToDomain(this JobTemplateStepDto dto)
    {
        var createProcessorName = ProcessorName.Create(dto.ProcessorName);
        if (createProcessorName.IsFailure)
        {
            return createProcessorName.Error;
        }

        var sinks = dto.Sinks.Select(sink => new JobTemplateStepSink
        {
            Name = sink.Key,
            Source = sink.Value
        }).ToArray();

        var sources = dto.Sources.Select(source => new JobTemplateStepSource
        {
            Name = source.Key,
            Destination = source.Value
        }).ToArray();

        var properties = dto.Properties.Select(property => new JobTemplateStepProperty
        {
            Name = property.Key,
            Value = property.Value
        }).ToArray();

        var step = new JobTemplateStep
        {
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Description = dto.Description,
            ProcessorName = createProcessorName.Value,
            Sinks = sinks,
            Sources = sources,
            Properties = properties
        };

        return Result.Created(step);
    }
}
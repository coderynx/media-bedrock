using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Domain.JobTemplates.Manifests;
using MediaBedrock.Cli.Domain.Processors;
using MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

namespace MediaBedrock.Cli.Presentation.JobTemplates.Mappers;

internal static class JobTemplateStepMappers
{
    public static JobTemplateManifestStep ToDomain(this JobTemplateManifestStepDto dto)
    {
        var stepName = new JobTemplateStepName(dto.Name);
        var processorName = ProcessorName.Create(dto.ProcessorName).Value;

        return new JobTemplateManifestStep(
            name: stepName,
            processorName: processorName,
            sinksMappings: dto.Sinks,
            sourcesMappings: dto.Sources,
            properties: dto.Properties,
            displayName: dto.DisplayName,
            description: dto.Description);
    }
}
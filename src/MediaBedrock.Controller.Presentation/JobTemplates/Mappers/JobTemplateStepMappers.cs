using MediaBedrock.Controller.Domain.JobTemplates;
using MediaBedrock.Controller.Domain.JobTemplates.Manifests;
using MediaBedrock.Controller.Domain.Processing;
using MediaBedrock.Controller.Presentation.JobTemplates.Contracts;

namespace MediaBedrock.Controller.Presentation.JobTemplates.Mappers;

internal static class JobTemplateStepMappers
{
    public static JobTemplateManifestStep ToDomain(this JobTemplateManifestStepDto dto)
    {
        var stepName = new JobTemplateStepName(dto.Name);
        var processorName = ProcessorName.Create(dto.ProcessorName).Value;

        return new JobTemplateManifestStep(
            name: stepName,
            processorName: processorName,
            inputsMappings: dto.Inputs,
            outputsMappings: dto.Outputs,
            properties: dto.Properties,
            displayName: dto.DisplayName,
            description: dto.Description);
    }
}
using MediaBedrock.Controller.Domain.JobTemplates;
using MediaBedrock.Controller.Domain.JobTemplates.Manifests;
using MediaBedrock.Controller.Presentation.JobTemplates.Contracts;

namespace MediaBedrock.Controller.Presentation.JobTemplates.Mappers;

public static class JobTemplateMappers
{
    public static JobTemplateManifest ToDomain(this JobTemplateManifestDto dto)
    {
        return new JobTemplateManifest(
            name: new JobTemplateName(dto.Name),
            version: new JobTemplateVersion(dto.Version),
            author: new JobTemplateAuthor(dto.Author),
            inputs: dto.Inputs
                .Select(i => new JobTemplateManifestInput(i.Name, i.DisplayName, i.Description))
                .ToList(),
            outputs: dto.Outputs
                .Select(o => new JobTemplateManifestOutput(o.Name, o.DisplayName, o.Description))
                .ToList(),
            properties: dto.Properties
                .Select(p => new JobTemplateManifestProperty(p.Name, p.DisplayName, p.Description, p.DefaultValue))
                .ToList(),
            steps: dto.Steps
                .Select(s => s.ToDomain())
                .ToList(),
            displayName: new JobTemplateDisplayName(dto.DisplayName),
            description: new JobTemplateDescription(dto.Description));
    }
}
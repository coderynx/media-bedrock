using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

namespace MediaBedrock.Cli.Presentation.JobTemplates.Mappers;

internal static class JobTemplatePropertyMapper
{
    public static JobTemplateProperty ToDomain(this JobTemplatePropertyDto dto)
    {
        return new JobTemplateProperty
        {
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            DefaultValue = dto.DefaultValue,
            Description = dto.Description
        };
    }
}
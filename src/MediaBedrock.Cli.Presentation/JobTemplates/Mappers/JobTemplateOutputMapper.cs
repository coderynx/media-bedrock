using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

namespace MediaBedrock.Cli.Presentation.JobTemplates.Mappers;

internal static class JobTemplateOutputMapper
{
    public static JobTemplateOutput ToDomain(this JobTemplateOutputDto dto)
    {
        return new JobTemplateOutput
        {
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Description = dto.Description
        };
    }
}
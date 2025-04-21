using MediaBedrock.Cli.Domain.JobTemplates;
using MediaBedrock.Cli.Presentation.JobTemplates.Contracts;

namespace MediaBedrock.Cli.Presentation.JobTemplates.Mappers;

internal static class JobTemplateInputMapper
{
    public static JobTemplateInput ToDomain(this JobTemplateInputDto dto)
    {
        return new JobTemplateInput
        {
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Description = dto.Description
        };
    }
}